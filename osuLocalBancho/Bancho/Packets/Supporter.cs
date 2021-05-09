
using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Bancho.Packets
{
    public class Supporter : DataPacket
    {
        public override ServerPacketType Type => ServerPacketType.Supporter;

        private readonly UserRanks rank;

        public Supporter(UserRanks _rank)
        {
            rank = _rank;
        }

        public override byte[] GetBytes()
        {
            return PackData((uint)rank);
        }
    }
}
