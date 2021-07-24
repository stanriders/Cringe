using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class MainMenuIcon : ResponsePacket
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