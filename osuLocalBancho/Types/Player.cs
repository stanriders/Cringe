﻿
namespace osuLocalBancho.Types
{
    public class Player
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public float Accuracy { get; set; }
        public uint Playcount { get; set; }
        public ulong TotalScore { get; set; }
        public uint Rank { get; set; }
        public ushort Pp { get; set; }

        public static Player DummyPlayer => new Player
        {
            Id = 1,
            Username = "osuHOW",
            Accuracy = 0.9999f,
            Playcount = 123456789,
            TotalScore = 999999999,
            Rank = 1,
            Pp = 11727
        };
    }
}
