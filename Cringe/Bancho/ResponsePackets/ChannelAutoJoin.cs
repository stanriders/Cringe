using Cringe.Types.Bancho;
using Cringe.Types.Enums;

namespace Cringe.Bancho.ResponsePackets
{
    public class ChannelAutoJoin : ChannelInfo
    {
        public ChannelAutoJoin(Chat chat) : base(chat)
        {
        }

        public override ServerPacketType Type => ServerPacketType.ChannelAutoJoin;
    }
}