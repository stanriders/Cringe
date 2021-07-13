namespace Cringe.Types
{
    public class Stats 
    {
        public uint UserId { get; set; }
        public ChangeAction Action { get; set; }
        public ulong RankedScore { get; set; }
        public float Accuracy { get; set; }
        public uint Playcount { get; set; }
        public ulong TotalScore { get; set; }
        public uint GameRank { get; set; }
        public ushort Pp { get; set; }
    }
}