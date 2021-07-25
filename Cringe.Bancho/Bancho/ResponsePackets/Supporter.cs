using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class Supporter : ResponsePacket
    {
        private readonly UserRanks rank;

        public Supporter(UserRanks _rank)
        {
            rank = _rank;
        }

        public override ServerPacketType Type => ServerPacketType.Privileges;

        public override byte[] GetBytes()
        {
            return PackData((uint) rank);
        }
    }
}
