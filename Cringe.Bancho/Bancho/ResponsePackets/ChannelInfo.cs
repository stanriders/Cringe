using System.Linq;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class ChannelInfo : ResponsePacket
    {
        private readonly GlobalChat _chat;

        public ChannelInfo(GlobalChat chat)
        {
            _chat = chat;
        }

        public override ServerPacketType Type => ServerPacketType.ChannelInfo;

        public override byte[] GetBytes()
        {
            var data = PackData(_chat.Name)
                .Concat(PackData(_chat.Description))
                .Concat(PackData(_chat.Count));

            return data.ToArray();
        }
    }
}
