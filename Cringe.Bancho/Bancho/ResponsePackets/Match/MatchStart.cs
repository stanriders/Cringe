using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Match;

public class MatchStart : MatchJoinSuccess
{
    public MatchStart(Types.Match match) : base(match)
    {
    }

    public override ServerPacketType Type => ServerPacketType.MatchStart;
}
