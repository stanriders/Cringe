using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets;

public class ChannelKick : ChannelInfo
{
    public ChannelKick(GlobalChat chat) : base(chat)
    {
    }

    public override ServerPacketType Type => ServerPacketType.ChannelKick;
}
