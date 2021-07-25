using System;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class PartLobby : RequestPacket
    {
        public PartLobby(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.PartLobby;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            Logger.LogDebug("{Token} | Logged out the lobby", session.Token);
            ChatService.GetChat(ChatService.LobbyName)?.Disconnect(session);
            Lobby.Disconnect(session);

            return Task.CompletedTask;
        }
    }
}
