using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets;

public class Restart : ResponsePacket
{
    private readonly int _ms;

    public Restart(int ms)
    {
        _ms = ms;
    }

    public override ServerPacketType Type => ServerPacketType.Restart;

    public override byte[] GetBytes()
    {
        return PackData(_ms);
    }
}
