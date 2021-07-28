using System;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class MatchSkip : ResponsePacket
    {
        public override ServerPacketType Type => ServerPacketType.MatchSkip;

        public override byte[] GetBytes()
        {
            return Array.Empty<byte>();
        }
    }
}
