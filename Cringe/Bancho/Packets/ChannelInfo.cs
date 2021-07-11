﻿using System.Linq;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class ChannelInfo : DataPacket
    {
        public override ServerPacketType Type => ServerPacketType.ChannelInfo;
        private readonly Chat _chat;

        public ChannelInfo(Chat chat)
        {
            _chat = chat;
        }

        public override byte[] GetBytes()
        {
            var data = PackData(_chat.Name)
                .Concat(PackData(_chat.Description))
                .Concat(PackData(_chat.Users.Count));
            return data.ToArray();
        }
    }
}