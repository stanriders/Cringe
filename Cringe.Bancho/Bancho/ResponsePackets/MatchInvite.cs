using Cringe.Types.Database;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class MatchInvite : Message
    {
        public override ServerPacketType Type => ServerPacketType.MatchInvite;

        public MatchInvite(Player sender, string receiver, string message) : base(message, sender, receiver)
        {
        }
    }
}
