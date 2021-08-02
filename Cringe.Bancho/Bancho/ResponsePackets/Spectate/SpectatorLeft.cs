using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets.Spectate
{
    public class SpectatorLeft : ResponsePacket
    {
        private readonly int _id;

        public SpectatorLeft(int id)
        {
            _id = id;
        }

        public override ServerPacketType Type => ServerPacketType.SpectatorLeft;

        public override byte[] GetBytes()
        {
            return PackData(_id);
        }
    }
}
