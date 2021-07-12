using System;
using System.IO;
using System.Text;
using Cringe.Types.Enums;
using LEB128;

namespace Cringe.Bancho
{
    public abstract class DataPacket
    {
        public abstract ServerPacketType Type { get; }

        public abstract byte[] GetBytes();

        protected static byte[] PackData(short data)
        {
            return BitConverter.GetBytes(data);
        }

        protected static byte[] PackData(ushort data)
        {
            return BitConverter.GetBytes(data);
        }

        protected static byte[] PackData(int data)
        {
            var bytes = new byte[] {0xE2, 0x9C, 0x00, 0x00,};
            BitConverter.ToInt16(bytes, 0);
            return BitConverter.GetBytes(data);
        }

        protected static byte[] PackData(uint data)
        {
            return BitConverter.GetBytes(data);
        }

        protected static byte[] PackData(long data)
        {
            return BitConverter.GetBytes(data);
        }

        protected static byte[] PackData(ulong data)
        {
            return BitConverter.GetBytes(data);
        }

        protected static byte[] PackData(string data)
        {
            using var stream = new MemoryStream();
            stream.WriteByte(0x0B);
            stream.WriteLEB128Unsigned((ulong) Encoding.Latin1.GetByteCount(data));
            stream.Write(Encoding.Latin1.GetBytes(data));

            return stream.ToArray();
        }

        protected static byte[] PackData(int[] data)
        {
            using var stream = new MemoryStream();
            stream.Write(BitConverter.GetBytes((uint) data.Length));
            foreach (var integer in data) stream.Write(BitConverter.GetBytes(integer));
            return stream.ToArray();
        }

        protected static byte[] PackData(float data)
        {
            return BitConverter.GetBytes(data);
        }

        public static string ReadString(MemoryStream stream)
        {
            stream.ReadByte();
            var len = (int) stream.ReadLEB128Unsigned();
            var buffer = new byte[len];
            stream.Read(buffer, 0, len);
            return Encoding.Latin1.GetString(buffer);
        }
    }
}