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
        public string Password { get; set; }

        public Panel Panel =>
            new()
            {
                UserId = Id,
                Username = Username,
                Timezone = 24,
                Country = 0,
                UserRank = UserRank,
                Longitude = 0.0f,
                Latitude = 0.0f,
                GameRank = Rank
            };

        public Stats Stats =>
            new()
            {
                UserId = (uint) Id,
                ActionId = 0,
                ActionText = "",
                ActionMd5 = "",
                ActionMods = 0,
                GameMode = 0,
                BeatmapId = 0,
                RankedScore = TotalScore,
                Accuracy = Accuracy,
                Playcount = Playcount,
                TotalScore = TotalScore,
                GameRank = Rank,
                Pp = Pp
            };


        public static Player DummyPlayer => new()
        {
            Accuracy = 1.0f,
            Playcount = 0,
            TotalScore = 0,
            Rank = 1,
            Pp = 0,
            UserRank = UserRanks.Normal | UserRanks.Supporter | UserRanks.BAT | UserRanks.TournamentStaff |
                       UserRanks.Peppy
        };

        public static Player Generate(string username, string password)
        {
            var player = DummyPlayer;
            player.Username = username;

            //TODO: crypt the password 
            player.Password = password;
            return player;
        }
    }
}