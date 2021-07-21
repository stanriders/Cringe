using System.IO;
using Cringe.Types;
using Cringe.Types.Bancho;
using Cringe.Types.Enums;

namespace Cringe.Bancho.ResponsePackets
{
    public class UserPresence : ResponsePacket
    {
        private readonly Presence _presence;

        public UserPresence(Presence presence)
        {
            _presence = presence;
        }

        public override ServerPacketType Type => ServerPacketType.UserPresence;

        public override byte[] GetBytes()
        {
            using var panelStream = new MemoryStream();

            panelStream.Write(PackData(_presence.UserId));
            panelStream.Write(PackData(_presence.Username));
            panelStream.WriteByte(_presence.Timezone);
            panelStream.WriteByte(_presence.Country);
            panelStream.WriteByte((byte) _presence.UserRank);
            panelStream.Write(PackData(_presence.Longitude));
            panelStream.Write(PackData(_presence.Latitude));
            panelStream.Write(PackData(_presence.GameRank));

            return panelStream.ToArray();
        }
    }
}