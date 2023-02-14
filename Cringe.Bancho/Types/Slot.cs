using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Types;

public class Slot
{
    public int? PlayerId { get; set; }
    public SlotStatus Status { get; set; }
    public MatchTeams Team { get; set; }
    public Mods Mods { get; set; } = Mods.None;
    public bool Loaded { get; set; } = false;
    public bool Skipped { get; set; } = false;

    public void Wipe()
    {
        PlayerId = null;
        Status = SlotStatus.Open;
        Team = MatchTeams.Neutral;
        Mods = Mods.None;
    }

    public override string ToString()
    {
        return $"{PlayerId?.ToString() ?? "NO USER"}|{Status}|{Team}|{Mods}|{Loaded}|{Skipped}";
    }
}
