using System;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class SendPublicMessagePacket : RequestPacket
    {
        public SendPublicMessagePacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.SendPublicMessage;

        public override async Task Execute(PlayerSession session, byte[] data)
        {
            var dest = data[2..];
            var message = await Message.Parse(dest, session.Token.Username);
            Chats.SendGlobalMessage(message);
        }
    }
}