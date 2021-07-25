using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class ChannelPart : RequestPacket
    {
        public ChannelPart(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.ChannelPart;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            using var stream = new MemoryStream(data);
            var server = ReadString(stream);
            ChatService.GetChat(server)?.Disconnect(session);

            return Task.CompletedTask;
        }
    }
}
