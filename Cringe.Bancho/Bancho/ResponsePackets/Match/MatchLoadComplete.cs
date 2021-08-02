using System;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Match
{
    public class MatchLoadComplete : ResponsePacket
    {
        public override ServerPacketType Type => ServerPacketType.MatchAllPlayersLoaded;

        public override byte[] GetBytes()
        {
            return Array.Empty<byte>();
        }
    }
}
