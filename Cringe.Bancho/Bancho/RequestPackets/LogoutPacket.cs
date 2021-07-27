using System;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class LogoutPacket : RequestPacket
    {
        public LogoutPacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.Logout;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            Pool.Disconnect(session.Token);
            ChatService.Purge(session);

            Logger.LogInformation("{Token} | User logged out.\nConnected users are\n{Users}", session.Token,
                string.Join(",", PlayersPool.GetPlayersId()));

            return Task.CompletedTask;
        }
    }
}
