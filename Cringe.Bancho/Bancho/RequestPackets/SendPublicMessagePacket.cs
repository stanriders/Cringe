using System;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets
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
            if (message.Receiver == "#multiplayer")
            {
                if (session.MatchSession is not null)
                {
                    foreach (var slot in session.MatchSession.Match.Slots)
                        slot.Player?.ReceiveMessage(message);
                }
            }
            Chats.SendGlobalMessage(message);
        }
    }
}