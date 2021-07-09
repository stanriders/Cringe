using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class Supporter : DataPacket
    {
        private readonly UserRanks rank;

        public Supporter(UserRanks _rank)
        {
            rank = _rank;
        }

        public override ServerPacketType Type => ServerPacketType.Supporter;

        public override byte[] GetBytes()
        {
            return PackData((uint) rank);
        }
    }
}