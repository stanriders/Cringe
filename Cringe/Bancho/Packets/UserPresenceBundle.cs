using System.Collections.Generic;
using System.Linq;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class UserPresenceBundle : DataPacket
    {
        public override ServerPacketType Type => ServerPacketType.UserPresenceBundle;
        private readonly IEnumerable<int> _userIds;

        public UserPresenceBundle(IEnumerable<int> userIds)
        {
            _userIds = userIds;
        }

        public override byte[] GetBytes()
        {
            return _userIds.Select(PackData).Aggregate((acc, val) => acc.Concat(val).ToArray());
        }
    }
}