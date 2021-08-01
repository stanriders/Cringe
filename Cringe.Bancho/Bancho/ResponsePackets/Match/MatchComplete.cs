using System;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Match
{
    public class MatchComplete : ResponsePacket
    {
        public override ServerPacketType Type => ServerPacketType.MatchComplete;

        public override byte[] GetBytes()
        {
            return Array.Empty<byte>();
        }
    }
}
