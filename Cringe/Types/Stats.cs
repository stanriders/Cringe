namespace Cringe.Types
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
    }
}