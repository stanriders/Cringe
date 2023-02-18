using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Spectate;

public class SpectateFrames : ResponsePacket
{
    private readonly byte[] _raw;

    public SpectateFrames(byte[] raw)
    {
        _raw = raw;
    }

    public override ServerPacketType Type => ServerPacketType.SpectateFrames;

    public override byte[] GetBytes()
    {
        return _raw;
    }
}
