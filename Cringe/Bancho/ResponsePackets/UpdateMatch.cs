using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.ResponsePackets
{
    public class UpdateMatch : MatchJoinSuccess
    {
        public override ServerPacketType Type => ServerPacketType.UpdateMatch;

        public UpdateMatch(Lobby match) : base(match)
        {
        }
    }
}