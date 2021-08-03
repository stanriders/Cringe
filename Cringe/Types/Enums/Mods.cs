using System;
using System.ComponentModel.DataAnnotations;

namespace Cringe.Types.Enums
{
    [Flags]
    public enum Mods
    {
        None = 0,

        [Display(Name = "NF")]
        NoFail = 1,

        [Display(Name = "EZ")]
        Easy = 2,

        [Display(Name = "TD")]
        TouchDevice = 4,

        [Display(Name = "HD")]
        Hidden = 8,

        [Display(Name = "HR")]
        HardRock = 16,

        [Display(Name = "SD")]
        SuddenDeath = 32,

        [Display(Name = "DT")]
        DoubleTime = 64,

        [Display(Name = "RX")]
        Relax = 128,

        [Display(Name = "HT")]
        HalfTime = 256,

        [Display(Name = "NC")]
        Nightcore = 512, // Only set along with DoubleTime. i.e: NC only gives 576

        [Display(Name = "FL")]
        Flashlight = 1024,

        Autoplay = 2048,

        [Display(Name = "SO")]
        SpunOut = 4096,

        [Display(Name = "AP")]
        Relax2 = 8192,

        [Display(Name = "PF")]
        Perfect = 16384, // Only set along with SuddenDeath. i.e: PF only gives 16416

        Key4 = 32768,
        Key5 = 65536,
        Key6 = 131072,
        Key7 = 262144,
        Key8 = 524288,
        FadeIn = 1048576,
        Random = 2097152,
        LastMod = 4194304,
        Key9 = 16777216,
        Key10 = 33554432,
        Key1 = 67108864,
        Key3 = 134217728,
        Key2 = 268435456,

        [Display(Name = "V2")]
        V2 = 536870912,

        SpeedChangingMods = DoubleTime | Nightcore | HalfTime
    }
}
