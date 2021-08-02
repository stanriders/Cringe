using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Types
{
    public class MatchModel
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int Host { get; set; }
        public int MapId { get; set; }
        public string MapMd5 { get; set; }
        public string MapName { get; set; }
        public GameModes Mode { get; set; } = GameModes.Osu;
        public bool FreeMode { get; set; }
        public MatchWinConditions WinConditions { get; set; } = MatchWinConditions.score;
        public MatchTeamTypes TeamTypes { get; set; } = MatchTeamTypes.head_to_head;
        public bool InProgress { get; set; }
        public Mods Mods { get; set; }
    }
}
