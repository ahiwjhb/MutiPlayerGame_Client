#nullable enable
using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading.Channels;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Google.Protobuf;
using Logger;
using Network.SteamEncipherer;

using CRCType = System.UInt16;

namespace Network.MessageChannel
{
    public sealed class MessageChannel : IDisposable
    {
        private readonly static ConcurrentDictionary<string, MessageParser> _messageTypeNameToParserMapping = new ConcurrentDictionary<string, MessageParser>();

        private readonly Dictionary<Type, IEventNotifier> _messageTypeToRecivedEventMapping = new Dictionary<Type, IEventNotifier>();

        private readonly CancellationTokenSource _disposeCancellationTokenSource = new CancellationTokenSource();

        private readonly Channel<IMessage> _sendMessageQueue;

        private Socket? _socket = null!;

        private MessageBuffer _reciveMessageBuffer = null!;

        private bool _disposedValue = false;

        public EndPoint ConnectPoint { get; private set; } = null!;

        public bool Connected => !_disposedValue && _socket != null && _socket.Connected;

        private ILogger Logger { get;} 

        private IStreamEncipherer StreamEncipherer { get; }

        public MessageChannel(ILogger logger, IStreamEncipherer streamEncipherer) {
            Logger = logger;    
            StreamEncipherer = streamEncipherer;
            _sendMessageQueue = Channel.CreateBounded<IMessage>(new BoundedChannelOptions(capacity: 50) {
                SingleReader = true,
            });
        }

        ~MessageChannel() {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        public void StartListen(Socket socket, int reciveBufferSize = 4096) {
            if (_disposedValue) {
                throw new ObjectDisposedException(nameof(MessageChannel));
            }

            _socket = socket;
            _reciveMessageBuffer = new MessageBuffer(reciveBufferSize);
            ConnectPoint = socket.RemoteEndPoint;
            Task.Run(ReciveMessagesLoop);
            Task.Run(SendMessageLoop);
        }

        public void Dispose() {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void AddMessageListener<TMessage>(EventHandler<TMessage> callback, bool executeInMainThread = false) where TMessage : IMessage {
            var messageType = typeof(TMessage);

            _messageTypeNameToParserMapping.GetOrAdd(messageType.Name, _ => {
                PropertyInfo propertyInfo = messageType.GetProperty("Parser", BindingFlags.Public | BindingFlags.Static);
                return (MessageParser)propertyInfo.GetValue(null);
            });

            if (_messageTypeToRecivedEventMapping.TryGetValue(messageType, out var eventNotifier) == false) {
                eventNotifier = new EventNotifier<TMessage>(Logger);
                _messageTypeToRecivedEventMapping.Add(messageType, eventNotifier);
            }
            var recivedMessageEvent = (EventNotifier<TMessage>)eventNotifier;
            recivedMessageEvent.AddListener(callback);
        }

        public void RemoveMessageListener<TMessage>(EventHandler<TMessage> callback) where TMessage : IMessage {
            if (_messageTypeToRecivedEventMapping.TryGetValue(typeof(TMessage), out var eventNotifier)) {
                var recivedMessageEvent = (EventNotifier<TMessage>)eventNotifier;
                recivedMessageEvent.RemoveListenner(callback);
            }
        }

        public TMessage WaitMessage<TMessage>() where TMessage : class, IMessage{
            TMessage? message = null;
            void handler(MessageChannel _, TMessage result) => message = result;
            AddMessageListener<TMessage>(handler);
            try {
                while (message == null) {
                    if (Connected == false) {
                        throw new OperationCanceledException();
                    }
                }
            }
            finally {
                RemoveMessageListener<TMessage>(handler);
            }
            return message;
        }

        public async Task<TMessage> WaitMessageAsync<TMessage>(CancellationToken cancellationToken = default) where TMessage : class, IMessage {
            TMessage? message = null;
            void handler(MessageChannel _, TMessage result) => message = result;
            AddMessageListener<TMessage>(handler);
            try {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(_disposeCancellationTokenSource.Token, cancellationToken)) {
                    while (message == null) {
                        cts.Token.ThrowIfCancellationRequested();
                        await Task.Yield();
                    }
                }
            }
            finally {
                RemoveMessageListener<TMessage>(handler);
            }
            return message;
        }

        public void SendMessageAsync(IMessage message) {
            if (Connected == false) return;

            Task.Run(async () => {
                await _sendMessageQueue.Writer.WriteAsync(message, _disposeCancellationTokenSource.Token);
            });
        }

        private async void SendMessageLoop() {
            try {
                await foreach(var message in _sendMessageQueue.Reader.ReadAllAsync()) {
                    await SendMessageAsync_Internal(message);
                    Logger.LogFormat("已向{0}发送{1}包", ConnectPoint, message.GetType().Name);
                }
            }
            catch (OperationCanceledException) {
                // do nothing;
            }
            catch(Exception e) {
                Logger.ErrorFormat("消息发送异常! {0}", e.Message);
            }
            finally {
                Logger.LogFormat("消息发送通道关闭{0}", ConnectPoint);
            }
        }

        private async ValueTask SendMessageAsync_Internal(IMessage message) {
            if (Connected == false) return;

            // [包头](包体长度) + [包体]( 消息类型 + 消息本体 + crc校验码 )
            int packbodyLength = CodedOutputStream.ComputeStringSize(message.GetType().Name) + message.CalculateSize() + sizeof(CRCType);
            int packheadLength = CodedOutputStream.ComputeLengthSize(packbodyLength);
            int packLength = packheadLength + packbodyLength;
            var messageBuffer = ArrayPool<byte>.Shared.Rent(packLength);
            try {
                using var writer = new CodedOutputStream(messageBuffer);
                writer.WriteLength(packLength);
                writer.WriteString(message.GetType().Name);
                writer.WriteRawMessage(message);

                int writedIndex = (int)writer.Position;
                CRCType packCRC = CRC.CaculateCRC16(messageBuffer.AsSpan(0, writedIndex));
                BitConverter.TryWriteBytes(messageBuffer.AsSpan(writedIndex), packCRC);
                StreamEncipherer.Encryption(messageBuffer.AsSpan(0, packLength));

                await _socket.SendAsync(messageBuffer.AsMemory(0, packLength), SocketFlags.None, _disposeCancellationTokenSource.Token);
            }
            finally {
                ArrayPool<byte>.Shared.Return(messageBuffer);
            }
        }

        private async void ReciveMessagesLoop() {
            try {
                while (Connected) {
                    await ParseBufferToMessageAsync();
                }
            }
            catch (ObjectDisposedException) {
                // do nothing
            }
            catch (Exception e) {
                Logger.ErrorFormat("消息接收异常! {0}", e.Message);
            }
            finally {
                Logger.LogFormat("消息接收通道关闭{0}", ConnectPoint);
            }
        }

        private async ValueTask ParseBufferToMessageAsync() {
            MessageBuffer reciveMessageBuffer = _reciveMessageBuffer!;
            int writeBegin = reciveMessageBuffer.UsedBytes;
            int recivedBytes = await _socket.ReceiveAsync(reciveMessageBuffer.GetUnUseSegment(), SocketFlags.None);

            if (recivedBytes == 0) {
                Dispose();
                return;
            }

            reciveMessageBuffer.UsedBytes += recivedBytes;
            StreamEncipherer.Decryption(reciveMessageBuffer[writeBegin..]);

            if (reciveMessageBuffer.UsedBytes <= sizeof(CRCType)) {
                return;
            }

            int readIndex = 0, bufferEnd = reciveMessageBuffer.UsedBytes;
            try {
                while (readIndex < bufferEnd) {
                    int packheadLength, packbodyLength, packLength;
                    using (var reader = reciveMessageBuffer.CreatReadStream(readIndex..bufferEnd)) {
                        packLength = reader.ReadLength();
                        packheadLength = CodedOutputStream.ComputeLengthSize(packLength);
                        packbodyLength = packLength - packheadLength;
                    }
                    if (packLength == 0 || packLength > bufferEnd - readIndex) {
                        break;
                    }

                    int packbodyBegin = readIndex + packheadLength;
                    int crcEnd = packbodyBegin + packbodyLength, crcBegin = crcEnd - sizeof(CRCType);
                    CRCType packCRC = BitConverter.ToUInt16(reciveMessageBuffer[crcBegin..crcEnd]);
                    CRCType caculateCRC = CRC.CaculateCRC16(reciveMessageBuffer[readIndex..crcBegin]);
                    if (packCRC == caculateCRC) {
                        using (var reader = reciveMessageBuffer.CreatReadStream(packbodyBegin..crcBegin)) {
                            string typeName = reader.ReadString();
                            if (_messageTypeNameToParserMapping.TryGetValue(typeName, out var parser)) {
                                IMessage message = parser.ParseFrom(reader);
                                SendMessageEvent(message);
                                Logger.LogFormat("接收到ip地址{0} {1}消息", ConnectPoint.ToString(), typeName);
                            }
                            else {
                                Logger.WarringFormat("找不到消息类型{0}对应的解析器", typeName);
                            }
                        }
                    }
                    else {
                        Logger.Warring("包体完整性校验失败");
                    }

                    readIndex += packLength;
                }
            }
            catch (InvalidProtocolBufferException e) {
                Logger.WarringFormat("包体解析失败{0}", e.Message);
            }
            finally {
                reciveMessageBuffer.PopFront(readIndex);
            }
        }

        private void SendMessageEvent(IMessage message) {
            if (_messageTypeToRecivedEventMapping.TryGetValue(message.GetType(), out var recivedMessageEvent)) {
                recivedMessageEvent.SafeInvokeToMultiThread(this, message);
            }
        }

        private void Dispose(bool disposing) {
            if (_disposedValue) return;

            _disposeCancellationTokenSource.Cancel();
            _disposeCancellationTokenSource.Dispose();
            _messageTypeToRecivedEventMapping.Clear();
            _sendMessageQueue.Writer.Complete(new OperationCanceledException());
            try {
                _socket?.Shutdown(SocketShutdown.Both);
                _socket?.Close();
            }
            catch (SocketException e) {
                Logger.Log(e.Message);
            }
            finally {
                _socket?.Dispose();
            }
            _disposedValue = true;
        }
    }
}
