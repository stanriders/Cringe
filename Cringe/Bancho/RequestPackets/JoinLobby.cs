using System;
using System.Threading.Tasks;
using Cringe.Services;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class JoinLobby : RequestPacket
    {
        public JoinLobby(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.JoinLobby;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            ChatService.GetChat(ChatService.LobbyName)?.Connect(session);
            return Task.CompletedTask;
        }
    }
}