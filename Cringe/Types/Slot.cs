using Cringe.Types.Database;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Types
{
    public class Slot
    {
        public int Index { get; set; }
        public Player Player { get; set; }
        public SlotStatus Status { get; set; }
        public MatchTeams Team { get; set; }
        public Mods Mods { get; set; } = Mods.None;
        public bool Loaded { get; set; } = false;
        public bool Skipped { get; set; } = false;
    }
}