using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Match
{
    public class DisposeMatch : MatchJoinSuccess
    {
        public DisposeMatch(Types.Match match) : base(match)
        {
        }

        public override ServerPacketType Type => ServerPacketType.DisposeMatch;
    }
}
