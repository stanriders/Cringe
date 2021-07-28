using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class MatchPlayerSkipped : ResponsePacket
    {
        private int _slot;

        public MatchPlayerSkipped(int slot)
        {
            _slot = slot;
        }

        public override ServerPacketType Type => ServerPacketType.MatchPlayerFailed;

        public override byte[] GetBytes()
        {
            return PackData(_slot);
        }
    }
}
