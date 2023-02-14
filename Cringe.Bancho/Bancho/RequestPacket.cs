using System.IO;
using System.Text;
using Cringe.Types.Enums;
using Cringe.Utils;

namespace Cringe.Bancho.Bancho;

public abstract class RequestPacket
{
    public abstract ClientPacketType Type { get; }

    public static string ReadString(Stream stream)
    {
        stream.ReadByte();
        var len = (int) stream.ReadLEB128Unsigned();
        var buffer = new byte[len];
        stream.Read(buffer, 0, len);

        return Encoding.Latin1.GetString(buffer);
    }
}
