
using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Bancho.Packets
{
    public class UserId : DataPacket
    {
        public override ServerPacketType Type => ServerPacketType.UserId;

        private readonly int id;

        public UserId(int _id)
        {
            id = _id;
        }

        public override byte[] GetBytes()
        {
            return PackData(id);
        }
    }
}
