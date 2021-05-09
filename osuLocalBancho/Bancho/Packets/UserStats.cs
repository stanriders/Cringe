
using System.IO;
using osuLocalBancho.Types;
using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Bancho.Packets
{
    public class UserStats : DataPacket
    {
        public override ServerPacketType Type => ServerPacketType.UserPanel;

        private readonly Stats stats;

        public UserStats(Stats _stats)
        {
            stats = _stats;
        }

        public override byte[] GetBytes()
        {
            using var statsStream = new MemoryStream();

            statsStream.Write(PackData(stats.UserId));
            statsStream.WriteByte(stats.ActionId);
            statsStream.Write(PackData(stats.ActionText));
            statsStream.Write(PackData(stats.ActionMd5));
            statsStream.Write(PackData(stats.ActionMods));
            statsStream.WriteByte(stats.GameMode);
            statsStream.Write(PackData(stats.BeatmapId));
            statsStream.Write(PackData(stats.RankedScore));
            statsStream.Write(PackData(stats.Accuracy));
            statsStream.Write(PackData(stats.Playcount));
            statsStream.Write(PackData(stats.TotalScore));
            statsStream.Write(PackData(stats.GameRank));
            statsStream.Write(PackData(stats.Pp));

            return statsStream.ToArray();
        }
    }
}
