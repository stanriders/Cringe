using System;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Match
{
    public class MatchJoinFail : ResponsePacket
    {
        public override ServerPacketType Type => ServerPacketType.MatchJoinFail;

        public override byte[] GetBytes()
        {
            return Array.Empty<byte>();
        }
    }
}
