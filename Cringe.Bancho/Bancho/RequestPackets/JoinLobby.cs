using System;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class JoinLobby : RequestPacket
    {
        public JoinLobby(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.JoinLobby;
        protected override string ApiPath => "user/channels/join";

        public override Task Execute(PlayerSession session, byte[] data)
        {
            Logger.LogInformation("{Token} | Logged in the lobby", session.Token);
            ChatService.GetChat(ChatService.LobbyName)?.Connect(session);
            Lobby.Connect(session);

            return Task.CompletedTask;
        }
    }
}
