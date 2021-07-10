using System.IO;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class UserPanel : DataPacket
    {
        private readonly Panel panel;

        public UserPanel(Panel _panel)
        {
            panel = _panel;
        }

        public override ServerPacketType Type => ServerPacketType.UserPresence;

        public override byte[] GetBytes()
        {
            using var panelStream = new MemoryStream();

            panelStream.Write(PackData(panel.UserId));
            panelStream.Write(PackData(panel.Username));
            panelStream.WriteByte(panel.Timezone);
            panelStream.WriteByte(panel.Country);
            panelStream.WriteByte((byte) panel.UserRank);
            panelStream.Write(PackData(panel.Longitude));
            panelStream.Write(PackData(panel.Latitude));
            panelStream.Write(PackData(panel.GameRank));

            return panelStream.ToArray();
        }
    }
}