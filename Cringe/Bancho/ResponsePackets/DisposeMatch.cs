using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.ResponsePackets
{
    public class DisposeMatch : MatchJoinSuccess
    {
        public override ServerPacketType Type => ServerPacketType.DisposeMatch;

        public DisposeMatch(Lobby match) : base(match)
        {
        }
    }
}