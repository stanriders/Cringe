using System;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

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
            Logger.LogInformation("{Token} | Sends message {Message}", session.Token, message);
            if (message.Receiver == "#multiplayer")
            {
                if (session.MatchSession is not null)
                {
                    foreach (var slot in session.MatchSession.Match.Slots)
                        slot.Player?.ReceiveMessage(message);
                }
                else
                    Logger.LogError("{Token} | Sends message to #multiplayer while his MatchSession is null", session.Token);
            }

            ChatService.SendGlobalMessage(message);
        }
    }
}
