using Cringe.Bancho.Bancho;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Types;

public class ChangeAction
{
    public ActionType Action
    {
        get => (ActionType) ActionId;
        set => ActionId = (byte) value;
    }

    [PeppyField]
    public byte ActionId { get; set; }

    [PeppyField]
    public string ActionText { get; set; }

    [PeppyField]
    public string ActionMd5 { get; set; }

    [PeppyField]
    public uint ActionMods { get; set; }

    [PeppyField]
    public byte GameMode { get; set; }

    [PeppyField]
    public int BeatmapId { get; set; }

    public override string ToString()
    {
        return $"Action {ActionId}|{ActionText}|{ActionMd5}|{ActionMods}|{GameMode}|{BeatmapId}";
    }
}
