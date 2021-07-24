using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class ChannelInfoEnd : ResponsePacket
    {
        public override ServerPacketType Type => ServerPacketType.ChannelInfoEnd;

        public override byte[] GetBytes()
        {
            return PackData((uint) 0);
        }
    }
}