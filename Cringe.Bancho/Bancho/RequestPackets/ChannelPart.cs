using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class ChannelPart : RequestPacket
    {
        public ChannelPart(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.ChannelPart;
        protected override string ApiPath => "user/channels/part";

        public override async Task Execute(PlayerSession session, byte[] data)
        {
            await using var stream = new MemoryStream(data);
            var chat = ReadString(stream);
            Logger.LogInformation("{Token} | Leaves the {Chat} chat", session.Token, chat);
            ChatService.GetChat(chat)?.Disconnect(session);
        }
    }
}
