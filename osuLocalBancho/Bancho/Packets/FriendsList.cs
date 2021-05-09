
using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Bancho.Packets
{
    public class FriendsList : DataPacket
    {
        public override ServerPacketType Type => ServerPacketType.FriendsList;

        private readonly int[] friends;

        public FriendsList(int[] _friends)
        {
            friends = _friends;
        }

        public override byte[] GetBytes()
        {
            return PackData(friends);
        }
    }
}
