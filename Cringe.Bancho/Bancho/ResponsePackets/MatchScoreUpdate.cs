using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class MatchScoreUpdate : ResponsePacket
    {
        private readonly byte[] _bytes;

        public MatchScoreUpdate(byte[] bytes)
        {
            _bytes = bytes;
        }

        public override ServerPacketType Type => ServerPacketType.MatchScoreUpdate;

        public override byte[] GetBytes()
        {
            return _bytes;
        }
    }
}
