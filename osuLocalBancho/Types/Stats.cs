using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using osuLocalBancho.Bancho;

namespace osuLocalBancho.Types
{
    public class Stats
    {
        public uint UserId { get; set; }
        public byte ActionId { get; set; }
        public string ActionText { get; set; }
        public string ActionMd5 { get; set; }
        public int ActionMods { get; set; }
        public byte GameMode { get; set; }
        public int BeatmapId { get; set; }
        public ulong RankedScore { get; set; }
        public float Accuracy { get; set; }
        public uint Playcount { get; set; }
        public ulong TotalScore { get; set; }
        public uint GameRank { get; set; }
        public ushort Pp { get; set; }

        public byte[] Pack()
        {
            using var statsStream = new MemoryStream();

            statsStream.Write(DataPacket.PackData(UserId));
            statsStream.WriteByte(ActionId);
            statsStream.Write(DataPacket.PackData(ActionText));
            statsStream.Write(DataPacket.PackData(ActionMd5));
            statsStream.Write(DataPacket.PackData(ActionMods));
            statsStream.WriteByte(GameMode);
            statsStream.Write(DataPacket.PackData(BeatmapId));
            statsStream.Write(DataPacket.PackData(RankedScore));
            statsStream.Write(DataPacket.PackData(Accuracy));
            statsStream.Write(DataPacket.PackData(Playcount));
            statsStream.Write(DataPacket.PackData(TotalScore));
            statsStream.Write(DataPacket.PackData(GameRank));
            statsStream.Write(DataPacket.PackData(Pp));

            return statsStream.ToArray();
        }
    }
}
