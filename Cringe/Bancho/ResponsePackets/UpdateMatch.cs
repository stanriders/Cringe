using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.ResponsePackets
{
    public class UpdateMatch : MatchJoinSuccess
    {
        public UpdateMatch(Lobby match) : base(match)
        {
        }

        public override ServerPacketType Type => ServerPacketType.UpdateMatch;
    }
}