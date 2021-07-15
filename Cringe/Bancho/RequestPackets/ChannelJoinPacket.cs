using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class ChannelJoinPacket : RequestPacket
    {
        public ChannelJoinPacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.ChannelJoin;
        public override Task Execute(UserToken token, byte[] data)
        {
            using var stream = new MemoryStream(data[7..]);
            var str = ReadString(stream);
            Chats.Connect(token.PlayerId, str);
            return Task.CompletedTask;
        }
    }
}