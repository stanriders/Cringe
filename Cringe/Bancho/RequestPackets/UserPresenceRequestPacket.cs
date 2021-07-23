using System;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class UserPresenceRequestPacket : RequestPacket
    {
        public UserPresenceRequestPacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.UserPresenceRequest;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            session.Queue.EnqueuePacket(new UserPresence(session.Player.Presence));
            return Task.CompletedTask;
        }
    }
}