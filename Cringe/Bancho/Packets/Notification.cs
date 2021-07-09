using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class Notification : DataPacket
    {
        private readonly string text;

        public Notification(string _text)
        {
            text = _text;
        }

        public override ServerPacketType Type => ServerPacketType.Notification;

        public override byte[] GetBytes()
        {
            return PackData(text);
        }
    }
}