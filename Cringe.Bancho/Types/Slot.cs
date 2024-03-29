﻿using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Types
{
    public class Slot
    {
        public PlayerSession Player { get; set; }
        public SlotStatus Status { get; set; }
        public MatchTeams Team { get; set; }
        public Mods Mods { get; set; } = Mods.None;
        public bool Loaded { get; set; } = false;
        public bool Skipped { get; set; } = false;

        public void Wipe()
        {
            Player = null;
            Status = SlotStatus.Open;
            Team = MatchTeams.Neutral;
            Mods = Mods.None;
        }

        public override string ToString()
        {
            return $"{Player?.Id.ToString() ?? "NO USER"}|{Status}|{Team}|{Mods}|{Loaded}|{Skipped}";
        }
    }
}
