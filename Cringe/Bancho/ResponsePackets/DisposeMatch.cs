using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.ResponsePackets
{
    public class DisposeMatch : MatchJoinSuccess
    {
        public DisposeMatch(Match match) : base(match)
        {
        }

        public override ServerPacketType Type => ServerPacketType.DisposeMatch;
    }
}