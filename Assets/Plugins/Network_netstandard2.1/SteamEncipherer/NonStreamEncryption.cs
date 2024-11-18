using System;

namespace Network.SteamEncipherer
{
    public class NonStreamEncryption : IStreamEncipherer
     {
        public void Decryption(Span<byte> bytes) {
            // do nothing
        }

        public void Encryption(Span<byte> bytes) {
            // do nothing
        }
     }
}
