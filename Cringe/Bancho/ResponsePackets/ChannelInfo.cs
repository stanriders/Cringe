using System.Linq;
using Cringe.Types.Bancho;
using Cringe.Types.Enums;

namespace Cringe.Bancho.ResponsePackets
{
    public class ChannelInfo : ResponsePacket
    {
        private readonly Chat _chat;

        public ChannelInfo(Chat chat)
        {
            _chat = chat;
        }

        public override ServerPacketType Type => ServerPacketType.ChannelInfo;

        public override byte[] GetBytes()
        {
            var data = PackData(_chat.Name)
                .Concat(PackData(_chat.Description))
                .Concat(PackData(_chat.Users.Count));
            return data.ToArray();
        }
    }
}