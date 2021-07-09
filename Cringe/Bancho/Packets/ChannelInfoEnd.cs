using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class ChannelInfoEnd : DataPacket
    {
        public override ServerPacketType Type => ServerPacketType.ChannelInfoEnd;

        public override byte[] GetBytes()
        {
            return PackData((uint) 0);
        }
    }
}