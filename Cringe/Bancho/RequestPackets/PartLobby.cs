using System;
using System.Threading.Tasks;
using Cringe.Services;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class PartLobby : RequestPacket
    {
        public PartLobby(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.PartLobby;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            ChatService.GetChat(ChatService.LobbyName)?.Disconnect(session);
            return Task.CompletedTask;
        }
    }
}