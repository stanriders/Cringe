using System.Text.Json.Serialization;
using Cringe.Types.Database;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Types
{
    public class MatchModel
    {
        public short Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public int Host { get; set; }
        public int MapId { get; set; }
        public string MapMd5 { get; set; }
        public string MapName { get; set; }
        public GameModes Mode { get; set; } = GameModes.Osu;
        public bool FreeMode { get; set; }
        public MatchWinConditions WinConditions { get; set; } = MatchWinConditions.Score;
        public MatchTeamTypes TeamTypes { get; set; } = MatchTeamTypes.HeadToHead;
        public bool InProgress { get; set; }
        public Mods Mods { get; set; }

        public virtual Player[] Players { get; set; }
    }
}
