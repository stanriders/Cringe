using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Spectate
{
    public class SpectatorCantSpectate : ResponsePacket
    {
        private int _id;

        public SpectatorCantSpectate(int id)
        {
            _id = id;
        }

        public override ServerPacketType Type => ServerPacketType.SpectatorCantSpectate;

        public override byte[] GetBytes()
        {
            return PackData(_id);
        }
    }
}
