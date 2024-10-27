#nullable enable
using System.Net.Sockets;
using System.Net;
using Google.Protobuf;
using Network.Protocol;
using System.Threading;
using Network.MessageChannel;
using Cysharp.Threading.Tasks;

namespace MultiPlayerGame.Network
{
    public partial class Client 
    {
        private readonly MessageChannel _messageChannel;

        private readonly CancellationTokenSource _shutdownCancellationTokenSource = new();

        private readonly TimeoutController _timeoutController = new();

        public bool Connected => _messageChannel.Connected;

        public Client(MessageChannel messageChannel) {
            _messageChannel = messageChannel;
        }

        public void Start() {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7758));
             _messageChannel.StartListen(socket);

            new Thread(HeartbeatLoop) {
                IsBackground = true
            }.Start();
        }

        public void ShutdownConnect() {
            _messageChannel.Dispose();
            _shutdownCancellationTokenSource.Cancel();
            _shutdownCancellationTokenSource.Dispose();
        }

        public void SendMessageAsync(IMessage message) {
            _messageChannel.SendMessageAsync(message);
        }

        public void AddMessageListener<TMessage>(EventHandler<TMessage> handler) where TMessage : IMessage{
            _messageChannel.AddMessageListener(handler);
        }

        public void RemoveMessageListener<TMessage>(EventHandler<TMessage> handler) where TMessage : IMessage {
            _messageChannel.RemoveMessageListener(handler);
        }

        private void HeartbeatLoop() {
            const int loopCirleTimeMillisecond = 10000;
            while (Connected) {
                SendMessageAsync(HeartbeatPack.Empty);
                Thread.Sleep(loopCirleTimeMillisecond);
            }
        }
    }
}

namespace MultiPlayerGame.Network
{
    public partial class Client
    {
        public UniTask<RequestResult> RequsetLoginAsync(string username, string password, double timeout) {
            var request = new LoginRequest() {
                Username = username,
                Password = password,
            };

            SendMessageAsync(request);

            return WaitResultAsync(ActionCode.Login, timeout);
        }

        public UniTask<RequestResult> RequestRegisterAsync(string username, string password, double timeout) {
            var request = new RegisterRequest() {
                Username = username,
                Password = password,
            };

            SendMessageAsync(request);

            return WaitResultAsync(ActionCode.Register, timeout);
        }

        public UniTask<RequestResult> RequestCreatRoomAsync(int requesterID, string roomName, int maxPeopleLimit, double timeout) {
            var request = new CreateRoomRequest() {
                RequesterID = requesterID,
                RoomName = roomName,
                MaxPeopleLimit = maxPeopleLimit
            };

            SendMessageAsync(request);

            return WaitResultAsync(ActionCode.CreateRoom, timeout);
        }

        public UniTask<RequestResult> RequestFireAsync(int requesterID, double timeout) {
            var request = new FireRequest() {
                RequesterID = requesterID
            };

            SendMessageAsync(request);

            return WaitResultAsync(ActionCode.Attack, timeout);
        }

        public UniTask<RequestResult> RequestJoinRoomAsync(int requesterID, int roomID, double timeout) {
            var request = new JoinRoomRequest() {
                RequesterID = requesterID,
                JoinRoomID = roomID,
            };

            SendMessageAsync(request);

            return WaitResultAsync(ActionCode.JoinRoom, timeout);
        }

        public UniTask<RequestResult> RequestSearchRoomAsync(string roomName, double timeout) {
            var request = new SearchRoomRequest() {
                RoomName = roomName
            };

            SendMessageAsync(request);

            return WaitResultAsync(ActionCode.SearchRoom, timeout);
        }

        public UniTask<RequestResult> RequestUserInfoAsync(int userID, double timeout) {
            var request = new UserInfoRequest() {
                UserID = userID
            };

            SendMessageAsync(request);

            return WaitResultAsync(ActionCode.QueryUserInfo, timeout);
        }

        public UniTask<RequestResult> RequestExitRoomAsync(int requestID, int exitRoomID, double timeout) {
            var request = new ExitRoomRequest() {
                RequesterID = requestID,
                ExitRoomID = exitRoomID,
            };

            SendMessageAsync(request);

            return WaitResultAsync(ActionCode.ExitRoom, timeout);
        }

        private async UniTask<RequestResult> WaitResultAsync(ActionCode actionCode, double timeoutForSecond) {
            var timeLinit = System.TimeSpan.FromSeconds(timeoutForSecond);
            CancellationToken cancellationToken = _timeoutController.Timeout(timeLinit);
            try {
                RequestResult result;
                do {
                    result = await _messageChannel.WaitMessageAsync<RequestResult>(cancellationToken);
                }
                while (result.ActionCode != actionCode);

                return result;
            }
            catch (System.OperationCanceledException) {
                return RequestResultPackExtension.TimeOutRequest;
            }
        }
    }

    public static class RequestResultPackExtension
    {
        public static RequestResult TimeOutRequest { get; }

        static RequestResultPackExtension() {
            TimeOutRequest = new RequestResult() {
                ActionCode = ActionCode.None,
                IsSuccessful = false,
                Information = "服务请求超时"
            };
        }
    }
}