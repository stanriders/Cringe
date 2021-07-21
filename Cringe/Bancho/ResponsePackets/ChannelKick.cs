using Cringe.Types.Bancho;
using Cringe.Types.Enums;

namespace Cringe.Bancho.ResponsePackets
{
    public class ChannelKick : ChannelInfo 
    {
        public override ServerPacketType Type => ServerPacketType.ChannelKick;

        public ChannelKick(Chat chat) : base(chat)
        {
        }
    }
}