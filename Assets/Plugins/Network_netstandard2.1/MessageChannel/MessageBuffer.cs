using Google.Protobuf;
using System;

namespace Network.MessageChannel
{
    internal class MessageBuffer
    {
        internal delegate CodedOutputStream CreatCodedOutputStreamDelegate(byte[] bytes, int offset, int length);

        private readonly byte[] _buffer;

        private int _usedBytes = 0;

        public MessageBuffer(int bufferSize) {
            _buffer = new byte[bufferSize];
        }

        public int UsedBytes { 
            get => _usedBytes;
            internal set {
                _usedBytes = value;
                if (_usedBytes < 0 || _usedBytes > _buffer.Length) {
                    throw new ArgumentOutOfRangeException(nameof(MessageChannel) + " used bytes exceed the buffer size");
                }
            }
        }

        public void Clear() {
            UsedBytes = 0;
        }

        public Span<byte> this[Range range] {
            get => _buffer.AsSpan(range);
        }

        public CodedInputStream CreatReadStream(int begin, int length) {
            return CreatReadStream(begin..(begin + length));
        }

        public CodedInputStream CreatReadStream(Range range) {
            if (range.Start.IsFromEnd) {
                throw new ArgumentOutOfRangeException("the start index not be backward");
            }

            int begin = range.Start.Value;
            int end = range.End.IsFromEnd ? UsedBytes - range.End.Value : range.End.Value;
            return new CodedInputStream(_buffer, begin, end - begin);
        }


        internal ArraySegment<byte> GetUnUseSegment() {
            return new ArraySegment<byte>(_buffer, UsedBytes, _buffer.Length - UsedBytes);
        }

        internal Memory<byte> GetUnUseMemory(int size) {
            return _buffer.AsMemory(UsedBytes, size);
        }

        internal Memory<byte> AsMemory(Range range) {
            return _buffer.AsMemory(range);
        }

        internal Memory<byte> AsMemory(int offset, int length) {
            return _buffer.AsMemory(offset, length);
        }

        internal Span<byte> AsSpan(int offset, int length) {
            return _buffer.AsSpan(offset, length);
        }

        internal Span<byte> AsSpan(Range range) {
            return _buffer.AsSpan(range);
        }

        internal void PopFront(int count) {
            if (count > 0 && count <= UsedBytes) {
                UsedBytes -= count;
                Array.Copy(_buffer, count, _buffer, 0, UsedBytes);
            }
        }
    }
}
