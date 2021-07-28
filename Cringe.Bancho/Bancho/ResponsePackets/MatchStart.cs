using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class MatchStart : MatchJoinSuccess
    {
        public MatchStart(Match match) : base(match)
        {
        }

        public override ServerPacketType Type => ServerPacketType.MatchStart;
    }
}
