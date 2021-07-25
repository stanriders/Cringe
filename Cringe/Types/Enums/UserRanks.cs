using System;

namespace Cringe.Types.Enums
{
    [Flags]
    public enum UserRanks : byte
    {
        Normal = 0,
        Player = 1,
        BAT = 2,
        Supporter = 4,
        Moderator = 6,
        Peppy = 8,
        Admin = 16,
        TournamentStaff = 32
    }
}
