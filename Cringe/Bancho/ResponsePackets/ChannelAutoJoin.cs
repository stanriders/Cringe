using Cringe.Types;
using Cringe.Types.Bancho;
using Cringe.Types.Enums;

namespace Cringe.Bancho.ResponsePackets
{
    public class ChannelAutoJoin : ChannelInfo
    {
        public ChannelAutoJoin(GlobalChat chat) : base(chat)
        {
        }

        public override ServerPacketType Type => ServerPacketType.ChannelAutoJoin;
    }
}