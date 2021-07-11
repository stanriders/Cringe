using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class ChannelAutoJoin : ChannelInfo
    {
        public ChannelAutoJoin(Chat chat) : base(chat)
        {
        }

        public override ServerPacketType Type => ServerPacketType.ChannelAutoJoin;
    }
}