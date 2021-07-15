using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class ChannelPart : RequestPacket
    {
        public ChannelPart(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.ChannelPart;

        public override Task Execute(UserToken token, byte[] data)
        {
            using var stream = new MemoryStream(data[7..]);
            var server = ReadString(stream);
            Chats.Disconnect(token.PlayerId, server);
            return Task.CompletedTask;
        }
    }
}