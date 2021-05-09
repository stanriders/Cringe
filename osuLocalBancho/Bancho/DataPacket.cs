using System;
using System.IO;
using System.Text;
using LEB128;
using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Bancho
{
    public abstract class DataPacket
    {
        public virtual ServerPacketType Type { get; }

        public virtual byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        protected byte[] PackData(short data)
        {
            return BitConverter.GetBytes(data);
        }

        protected byte[] PackData(ushort data)
        {
            return BitConverter.GetBytes(data);
        }

        protected byte[] PackData(int data)
        {
            return BitConverter.GetBytes(data);
        }

        protected byte[] PackData(uint data)
        {
            return BitConverter.GetBytes(data);
        }

        protected byte[] PackData(long data)
        {
            return BitConverter.GetBytes(data);
        }

        protected byte[] PackData(ulong data)
        {
            return BitConverter.GetBytes(data);
        }

        protected byte[] PackData(string data)
        {
            using var stream = new MemoryStream();
            stream.WriteByte(0x0B);
            stream.WriteLEB128Unsigned((ulong)Encoding.Latin1.GetByteCount(data));
            stream.Write(Encoding.Latin1.GetBytes(data));

            return stream.ToArray();
        }

        protected byte[] PackData(int[] data)
        {
            using var stream = new MemoryStream();
            stream.Write(BitConverter.GetBytes((uint)data.Length));
            foreach (var integer in data)
            {
                stream.Write(BitConverter.GetBytes(integer));
            }
            return stream.ToArray();
        }

        protected byte[] PackData(float data)
        {
            return BitConverter.GetBytes(data);
        }
    }
}
