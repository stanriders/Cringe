﻿using System;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Match
{
    public class MatchTransferHost : ResponsePacket
    {
        public override ServerPacketType Type => ServerPacketType.MatchTransferHost;

        public override byte[] GetBytes()
        {
            return Array.Empty<byte>();
        }
    }
}
