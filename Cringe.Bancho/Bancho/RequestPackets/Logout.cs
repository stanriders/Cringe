using System;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class Logout : RequestPacket
    {
        public Logout(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.Logout;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (!Pool.Disconnect(session.Token))
                Logger.LogWarning("{Token} | Failed to disconnect", session.Token);

            session.MatchSession?.Disconnect(session);
            Spectate.NukeOrLogout(session);
            ChatService.Purge(session);
            Stats.RemoveStats(session.Id);

            Logger.LogInformation("{Token} | User logged out.\nConnected users are\n{Users}", session.Token,
                string.Join(",", PlayersPool.GetPlayersId()));

            return Task.CompletedTask;
        }
    }
}
