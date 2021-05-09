using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Bancho.Packets
{
    public class MainMenuIcon : DataPacket
    {
        public override ServerPacketType Type => ServerPacketType.MainMenuIcon;

        private readonly string link;

        public MainMenuIcon(string _link)
        {
            link = _link;
        }

        public override byte[] GetBytes()
        {
            return PackData(link);
        }
    }
}
