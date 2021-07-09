using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class MainMenuIcon : DataPacket
    {
        private readonly string link;

        public MainMenuIcon(string _link)
        {
            link = _link;
        }

        public override ServerPacketType Type => ServerPacketType.MainMenuIcon;

        public override byte[] GetBytes()
        {
            return PackData(link);
        }
    }
}