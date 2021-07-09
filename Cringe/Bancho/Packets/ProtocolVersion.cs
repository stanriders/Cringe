using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class ProtocolVersion : DataPacket
    {
        private readonly uint version;

        public ProtocolVersion(uint _version)
        {
            version = _version;
        }

        public override ServerPacketType Type => ServerPacketType.ProtocolVersion;

        public override byte[] GetBytes()
        {
            return PackData(version);
        }
    }
}