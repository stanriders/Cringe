using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Spectate;

public class FellowSpectatorLeft : ResponsePacket
{
    private readonly int _id;

    public FellowSpectatorLeft(int id)
    {
        _id = id;
    }

    public override ServerPacketType Type => ServerPacketType.FellowSpectatorLeft;

    public override byte[] GetBytes()
    {
        return PackData(_id);
    }
}
