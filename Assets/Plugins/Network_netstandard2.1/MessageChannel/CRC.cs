using System;

namespace Network.MessageChannel
{
    public static class CRC
    {
        private static readonly Lazy<ushort[]> _crc16Table;

        static CRC() {
            _crc16Table = new Lazy<ushort[]>(() => {
                const ushort polynomial = 0xA001;
                var table = new ushort[256];
                for (int i = 0; i < 256; i++) {
                    ushort crc = 0;
                    ushort temp = (ushort)i;
                    for (int j = 0; j < 8; j++) {
                        crc = (ushort)((((crc ^ temp) & 1) == 1) ? ((crc >> 1) ^ polynomial) : (crc >> 1));
                        temp >>= 1;
                    }
                    table[i] = crc;
                }
                return table;
            });
        }

        public static ushort CaculateCRC16(Span<byte> bytes) {
            ushort crc = 0xFFFF;
            foreach (byte b in bytes) {
                byte tableIndex = (byte)((crc ^ b) & 0xFF);
                crc = (ushort)((crc >> 8) ^ _crc16Table.Value[tableIndex]);
            }
            return crc;
        }
    }
}
