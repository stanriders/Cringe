using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class ChannelAutoJoin : ChannelInfo
    {
        public override ServerPacketType Type => ServerPacketType.ChannelAutoJoin;

        public ChannelAutoJoin(Chat chat) : base(chat)
        {
        }
    }
}