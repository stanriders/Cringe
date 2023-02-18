using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Match;

public class MatchJoinSuccess : ResponsePacket
{
    public MatchJoinSuccess(Types.Match match)
    {
        _match = match;
    }

    private Types.Match _match { get; }
    public override ServerPacketType Type => ServerPacketType.MatchJoinSuccess;

    public override byte[] GetBytes()
    {
        return PeppyConverter.Serialize(_match, true);
    }
}
