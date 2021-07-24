using System;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class JoinLobby : RequestPacket
    {
        public JoinLobby(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.JoinLobby;

        public override async Task Execute(PlayerSession session, byte[] data)
        {
            ChatService.GetChat(ChatService.LobbyName)?.Connect(session);
            await Lobby.Connect(session);
        }
    }
}