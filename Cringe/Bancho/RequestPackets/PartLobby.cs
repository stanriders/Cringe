using System;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
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
        public override Task Execute(UserToken token, byte[] data)
        {
            var lobby = Chat.Lobby;
            Chats.Disconnect(token.PlayerId, lobby.Name);
            return Task.CompletedTask;
        }
    }
}