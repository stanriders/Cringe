using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets;

public class SilenceEnd : ResponsePacket
{
    private readonly uint time;

    public SilenceEnd(uint _time)
    {
        time = _time;
    }

    public override ServerPacketType Type => ServerPacketType.SilenceEnd;

    public override byte[] GetBytes()
    {
        return PackData(time);
    }
}
