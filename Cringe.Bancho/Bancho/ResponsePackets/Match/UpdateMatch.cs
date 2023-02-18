using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Match;

public class UpdateMatch : MatchJoinSuccess
{
    public UpdateMatch(Types.Match match) : base(match)
    {
    }

    public override ServerPacketType Type => ServerPacketType.UpdateMatch;
}
