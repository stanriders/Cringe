using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class UpdateMatch : MatchJoinSuccess
    {
        public UpdateMatch(Match match) : base(match)
        {
        }

        public override ServerPacketType Type => ServerPacketType.UpdateMatch;
    }
}