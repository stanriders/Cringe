using System;
using System.Threading.Tasks;
using Cringe.Types;
using Cringe.Types.Bancho;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class JoinLobby : RequestPacket
    {
        public JoinLobby(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.JoinLobby;

        public override Task Execute(UserToken token, byte[] data)
        {
            var lobby = Chat.Lobby;
            Chats.Connect(token.PlayerId, lobby.Name);
            return Task.CompletedTask;
        }
    }
}