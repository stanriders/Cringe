using System;
using System.Threading.Tasks;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class LogoutPacket : RequestPacket
    {
        public LogoutPacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.Logout;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            Pool.Disconnect(session.Token.PlayerId);
            Chats.Purge(session);
            return Task.CompletedTask;
        }
    }
}