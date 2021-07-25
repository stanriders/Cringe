using System.IO;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class UserStats : ResponsePacket
    {
        private readonly Stats stats;

        public UserStats(Stats _stats)
        {
            stats = _stats;
        }

        public override ServerPacketType Type => ServerPacketType.UserStats;

        public override byte[] GetBytes()
        {
            using var statsStream = new MemoryStream();
            var action = stats.Action;
            statsStream.Write(PackData(stats.UserId));
            statsStream.WriteByte(action.ActionId);
            statsStream.Write(PackData(action.ActionText));
            statsStream.Write(PackData(action.ActionMd5));
            statsStream.Write(PackData(action.ActionMods));
            statsStream.WriteByte(action.GameMode);
            statsStream.Write(PackData(action.BeatmapId));
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
