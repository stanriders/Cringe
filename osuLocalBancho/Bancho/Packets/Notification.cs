using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Bancho.Packets
{
    public class Notification : DataPacket
    {
        public override ServerPacketType Type => ServerPacketType.Notification;

        private readonly string text;

        public Notification(string _text)
        {
            text = _text;
        }

        public override byte[] GetBytes()
        {
            return PackData(text);
        }
    }
}
