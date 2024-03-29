﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cringe.Types.Enums;

namespace Cringe.Types.Database
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public float Accuracy { get; set; }

        public uint Playcount { get; set; }
        public ulong TotalScore { get; set; }
        public uint Rank { get; set; }
        public ushort Pp { get; set; }
        public UserRanks UserRank { get; set; }
        public Bot Bot { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public static Player DummyPlayer => new()
        {
            Accuracy = 0.0f,
            Playcount = 0,
            TotalScore = 0,
            Rank = 0,
            Pp = 0,
            UserRank = UserRanks.Normal | UserRanks.Supporter | UserRanks.BAT | UserRanks.TournamentStaff |
                       UserRanks.Admin
        };

        public static Player Generate(string username, string password)
        {
            var player = DummyPlayer;
            player.UserRank = UserRanks.Normal | UserRanks.Supporter | UserRanks.BAT | UserRanks.TournamentStaff;
            player.Username = username;

            //TODO: crypt the password
            player.Password = password;

            return player;
        }
    }
}
