using Cringe.Types.Database;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Match
{
    public class MatchInvite : Message
    {
        public MatchInvite(Player sender, string receiver, string message) : base(message, sender, receiver)
        {
        }

        public override ServerPacketType Type => ServerPacketType.MatchInvite;
    }
}
