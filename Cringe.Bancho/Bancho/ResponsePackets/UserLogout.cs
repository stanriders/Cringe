using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class UserLogout : ResponsePacket
    {
        private readonly int _id;

        public UserLogout(int id)
        {
            _id = id;
        }

        public override ServerPacketType Type => ServerPacketType.UserLogout;

        public override byte[] GetBytes()
        {
            return PackData(_id);
        }
    }
}
