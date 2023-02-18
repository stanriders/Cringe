using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Types;

public class Slot
{
    public Slot()
    {
        Reset();
    }

    public Slot(int index) : this()
    {
        Status = index < 8 ? SlotStatus.Open : SlotStatus.Locked;
    }

    public void Reset()
    {
        PlayerId = null;
        Status = SlotStatus.Open;
        Mods = Mods.None;
        Team = MatchTeams.Neutral;
    }

    public bool HasPlayer()
    {
        return PlayerId.HasValue;
    }

    public int? PlayerId { get; set; }
    public SlotStatus Status { get; set; }
    public MatchTeams Team { get; set; }
    public Mods Mods { get; set; }

    public override string ToString()
    {
        return $"{PlayerId?.ToString() ?? "NO USER"}|{Status}|{Team}|{Mods}";
    }
}
