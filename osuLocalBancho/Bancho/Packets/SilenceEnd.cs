
using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Bancho.Packets
{
    public class SilenceEnd : DataPacket
    {
        public override ServerPacketType Type => ServerPacketType.SilenceEnd;

        private readonly uint time;

        public SilenceEnd(uint _time)
        {
            time = _time;
        }

        public override byte[] GetBytes()
        {
            return PackData(time);
        }
    }
}
