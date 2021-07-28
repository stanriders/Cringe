using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class MatchPlayerFailed : ResponsePacket
    {
        private readonly int _slot;

        public MatchPlayerFailed(int slot)
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
