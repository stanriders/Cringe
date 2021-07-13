using System.IO;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class UserStats : DataPacket
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

            statsStream.Write(PackData(stats.UserId));
            statsStream.WriteByte(stats.ActionId);
            if(!string.IsNullOrEmpty(stats.ActionText))
                statsStream.Write(PackData(stats.ActionText));
            else
                statsStream.WriteByte(0);
            if(!string.IsNullOrEmpty(stats.ActionMd5))
                statsStream.Write(PackData(stats.ActionMd5));
            else 
                statsStream.WriteByte(0);
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