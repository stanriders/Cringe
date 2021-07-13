using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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

        public static string ReadString(Stream stream)
        {
            stream.ReadByte();
            var len = (int) stream.ReadLEB128Unsigned();
            var buffer = new byte[len];
            stream.Read(buffer, 0, len);
            return Encoding.Latin1.GetString(buffer);
        }

        public static int ReadInt(byte[] data)
        {
            return BitConverter.ToInt32(data);
        }

        
        public static int[] ReadI32(BinaryReader reader)
        {
            reader.ReadBytes(3);
            var length = (reader.ReadInt32() - 2) / 4;
            reader.ReadBytes(2);
            var buffer = new int[length];
            for (var i = 0; i < length; i++)
            {
                buffer[i] = reader.ReadInt32();
            }

            return buffer;
        }
    }
}