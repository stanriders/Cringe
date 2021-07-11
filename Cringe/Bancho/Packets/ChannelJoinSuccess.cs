﻿using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class ChannelJoinSuccess : ChannelInfo
    {
        public ChannelJoinSuccess(Chat chat) : base(chat)
        {
        }

        public override ServerPacketType Type => ServerPacketType.ChannelJoinSuccess;
    }
}