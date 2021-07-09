using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class FriendsList : DataPacket
    {
        private readonly int[] friends;

        public FriendsList(int[] _friends)
        {
            friends = _friends;
        }

        public override ServerPacketType Type => ServerPacketType.FriendsList;

        public override byte[] GetBytes()
        {
            return PackData(friends);
        }
    }
}