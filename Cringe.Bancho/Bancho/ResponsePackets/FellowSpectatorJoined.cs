using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class FellowSpectatorJoined : ResponsePacket
    {
        private readonly int _id;

        public FellowSpectatorJoined(int id)
        {
            _id = id;
        }
        public override ServerPacketType Type => ServerPacketType.FellowSpectatorJoined;

        public override byte[] GetBytes()
        {
            return PackData(_id);
        }
    }
}
