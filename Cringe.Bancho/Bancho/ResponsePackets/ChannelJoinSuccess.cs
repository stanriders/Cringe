using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets;

public class ChannelJoinSuccess : ChannelInfo
{
    public ChannelJoinSuccess(GlobalChat chat) : base(chat)
    {
    }

    public override ServerPacketType Type => ServerPacketType.ChannelJoinSuccess;
}
