using System;
using System.IO;
using System.Text;
using LEB128;

namespace osuLocalBancho.Bancho
{
    public static class DataPacket
    {
        public static byte[] PackData(short data)
        {
            return BitConverter.GetBytes(data);
        }

        public static byte[] PackData(ushort data)
        {
            return BitConverter.GetBytes(data);
        }

        public static byte[] PackData(int data)
        {
            return BitConverter.GetBytes(data);
        }

        public static byte[] PackData(uint data)
        {
            return BitConverter.GetBytes(data);
        }

        public static byte[] PackData(long data)
        {
            return BitConverter.GetBytes(data);
        }

        public static byte[] PackData(ulong data)
        {
            return BitConverter.GetBytes(data);
        }

        public static byte[] PackData(string data)
        {
            using var stream = new MemoryStream();
            stream.WriteByte(0x0B);
            stream.WriteLEB128Unsigned((ulong)Encoding.Latin1.GetByteCount(data));
            stream.Write(Encoding.Latin1.GetBytes(data));

            return stream.ToArray();
        }

        public static byte[] PackData(int[] data)
        {
            using var stream = new MemoryStream();
            stream.Write(BitConverter.GetBytes((uint)data.Length));
            foreach (var integer in data)
            {
                stream.Write(BitConverter.GetBytes(integer));
            }
            return stream.ToArray();
        }

        public static byte[] PackData(float data)
        {
            return BitConverter.GetBytes(data);
        }
    }
}
