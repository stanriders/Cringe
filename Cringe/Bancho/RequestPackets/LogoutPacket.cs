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

        public override Task Execute(UserToken token, byte[] data)
        {
            Pool.Nuke(token.PlayerId);
            Chats.NukeUser(token.PlayerId);
            return Task.CompletedTask;
        }
    }
}