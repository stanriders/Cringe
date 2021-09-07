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
        protected override string ApiPath => "user/messages/send/public";

        public override async Task Execute(PlayerSession session, byte[] data)
        {
            var dest = data[2..];
            var message = await Message.Parse(dest, session.Token.Username);
            message.Sender = session.Player;

            Logger.LogInformation("{Token} | Sends message {Message}", session.Token, message);
            if (message.Receiver == GlobalChat.Multiplayer.Name)
            {
                if (session.MatchSession is not null)
                    foreach (var slot in session.MatchSession.Match.Slots)
                        slot.Player?.ReceiveMessage(message);
                else
                    Logger.LogError("{Token} | Sends message to #multiplayer while his MatchSession is null",
                        session.Token);
            }
            else if (message.Receiver == GlobalChat.Spectate.Name)
            {
                if (session.SpectateSession is not null)
                {
                    foreach (var viewer in session.SpectateSession.Viewers.Values)
                        viewer.ReceiveMessage(message);
                    session.SpectateSession.Host.ReceiveMessage(message);
                }
                else
                    Logger.LogError("{Token} | Sends message to #spectator while his SpectateSession is null", session.Token);
            }

            if (!ChatService.SendGlobalMessage(message))
                Logger.LogWarning("{Token} | User tries to send a message {Message}", session.Token, message);
        }
    }
}
