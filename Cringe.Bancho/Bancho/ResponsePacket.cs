using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cringe.Types.Enums;
using Cringe.Utils;

namespace Cringe.Bancho.Bancho;

public abstract class ResponsePacket
{
    public abstract ServerPacketType Type { get; }

    public abstract byte[] GetBytes();

    protected static byte[] PackData(short data)
    {
        return BitConverter.GetBytes(data);
    }

    protected static byte[] PackData(bool data)
    {
        return new[] {(byte) (data ? 1 : 0)};
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

    protected static byte[] ConcatData(params IEnumerable<byte>[] arrays)
    {
        return arrays.Aggregate((acc, it) => acc.Concat(it)).ToArray();
    }
}
