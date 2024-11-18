using System;

namespace Network.SteamEncipherer
{
    public interface IStreamEncipherer
    {
        public void Encryption(Span<byte> bytes);

        public void Decryption(Span<byte> bytes);

        public void Encryption(byte[] bytes)
        {
            Encryption(bytes.AsSpan());
        }

        public void Decryption(byte[] bytes)
        {
            Decryption(bytes.AsSpan());
        }
    }
}
