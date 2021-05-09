
using System.IO;
using osuLocalBancho.Types;
using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Bancho.Packets
{
    public class UserPanel : DataPacket
    {
        public override ServerPacketType Type => ServerPacketType.UserPanel;

        private readonly Panel panel;

        public UserPanel(Panel _panel)
        {
            panel = _panel;
        }

        public override byte[] GetBytes()
        {
            using var panelStream = new MemoryStream();

            panelStream.Write(PackData(panel.UserId));
            panelStream.Write(PackData(panel.Username));
            panelStream.WriteByte(panel.Timezone);
            panelStream.WriteByte(panel.Country);
            panelStream.WriteByte((byte)panel.UserRank);
            panelStream.Write(PackData(panel.Longitude));
            panelStream.Write(PackData(panel.Latitude));
            panelStream.Write(PackData(panel.GameRank));

            return panelStream.ToArray();
        }
    }
}
