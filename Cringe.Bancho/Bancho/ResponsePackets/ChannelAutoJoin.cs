using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets;

public class ChannelAutoJoin : ChannelInfo
{
    public ChannelAutoJoin(GlobalChat chat) : base(chat)
    {
    }

    public override ServerPacketType Type => ServerPacketType.ChannelAutoJoin;
}
