using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class SpectatorJoined : ResponsePacket
    {
        private readonly int _playerId;

        public SpectatorJoined(int playerId)
        {
            _playerId = playerId;
        }

        public override ServerPacketType Type => ServerPacketType.SpectatorJoined;

        public override byte[] GetBytes()
        {
            return PackData(_playerId);
        }
    }
}
