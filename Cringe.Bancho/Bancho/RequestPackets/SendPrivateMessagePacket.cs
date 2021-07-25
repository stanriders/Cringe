using System;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class SendPrivateMessagePacket : RequestPacket
    {
        public SendPrivateMessagePacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.SendPrivateMessage;

        public override async Task Execute(PlayerSession session, byte[] data)
        {
            var dest = data[2..];
            var message = await Message.Parse(dest, session.Token.Username);
            Chats.SendPrivateMessage(message);
        }
    }
}
