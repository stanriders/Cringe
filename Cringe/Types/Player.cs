using System.ComponentModel.DataAnnotations;
using Cringe.Types.Enums;

namespace Cringe.Types
{
    public class Player
    {
        [Key] public int Id { get; set; }

        public string Username { get; set; }
        public float Accuracy { get; set; }
        public uint Playcount { get; set; }
        public ulong TotalScore { get; set; }
        public uint Rank { get; set; }
        public ushort Pp { get; set; }
        public UserRanks UserRank { get; set; }

        public static Player DummyPlayer => new()
        {
            Username = "osuHOW",
            Accuracy = 0.9999f,
            Playcount = 123456789,
            TotalScore = 999999999,
            Rank = 1,
            Pp = 11727,
            UserRank = UserRanks.Normal | UserRanks.Supporter | UserRanks.BAT | UserRanks.TournamentStaff |
                       UserRanks.Peppy
        };

        public static Player Generate(string username)
        {
            var player = DummyPlayer;
            player.Username = username;
            return player;
        }
    }
}