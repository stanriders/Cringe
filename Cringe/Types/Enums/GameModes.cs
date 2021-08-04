using System.ComponentModel.DataAnnotations;
using Cringe.Attributes;

namespace Cringe.Types.Enums
{
    public enum GameModes
    {
        [BeatconnectNaming("all")]
        All = -1,

        [BeatconnectNaming("std")]
        [Display(Name = "osu!")]
        Osu = 0,

        [BeatconnectNaming("taiko")]
        [Display(Name = "taiko")]
        Taiko = 1,

        [BeatconnectNaming("ctb")]
        [Display(Name = "catch")]
        Catch = 2,

        [BeatconnectNaming("mania")]
        [Display(Name = "mania")]
        Mania = 3
    }
}
