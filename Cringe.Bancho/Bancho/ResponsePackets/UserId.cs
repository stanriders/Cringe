using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class UserId : ResponsePacket
    {
        private readonly int id;

        public UserId(int _id)
        {
            id = _id;
        }

        public override ServerPacketType Type => ServerPacketType.UserId;

        public override byte[] GetBytes()
        {
            return PackData(id);
        }
    }
}