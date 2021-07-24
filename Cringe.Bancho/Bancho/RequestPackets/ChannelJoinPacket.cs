using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class ChannelJoinPacket : RequestPacket
    {
        public ChannelJoinPacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.ChannelJoin;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            using var stream = new MemoryStream(data);
            var str = ReadString(stream);
            var chat = ChatService.GetChat(str);
            chat?.Connect(session);
            return Task.CompletedTask;
        }
    }
}