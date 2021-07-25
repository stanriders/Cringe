using System.ComponentModel.DataAnnotations;

namespace Cringe.Types.Enums
{
    public enum GameModes
    {
        [Display(Name = "osu!")]
        Osu = 0,

        [Display(Name = "taiko")]
        Taiko = 1,

        [Display(Name = "catch")]
        Catch = 2,

        [Display(Name = "mania")]
        Mania = 3,

        [Display(Name = "Relax osu!")]
        RxStd = 4,

        [Display(Name = "Relax taiko")]
        RxTaiko = 5,

        [Display(Name = "Relax catch")]
        RxCatch = 6,

        [Display(Name = "Autoplay Std")]
        ApStd = 7
    }
}
