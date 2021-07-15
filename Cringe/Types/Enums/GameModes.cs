using System.ComponentModel.DataAnnotations;

namespace Cringe.Types.Enums
{
    public enum GameModes
    {
        [Display(Name = "osu!")] Osu = 0,

        [Display(Name = "taiko")] Taiko = 1,

        [Display(Name = "catch")] Catch = 2,

        [Display(Name = "mania")] Mania = 3
    }
}