
using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Bancho.Packets
{
    public class ProtocolVersion : DataPacket
    {
        public override ServerPacketType Type => ServerPacketType.ProtocolVersion;

        private readonly uint version;

        public ProtocolVersion(uint _version)
        {
            version = _version;
        }

        public override byte[] GetBytes()
        {
            return PackData(version);
        }
    }
}
