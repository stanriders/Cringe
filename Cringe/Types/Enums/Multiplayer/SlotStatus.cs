using System;

namespace Cringe.Types.Enums.Multiplayer
{
    [Flags]
    public enum SlotStatus : byte
    {
        open = 1,
        locked = 2,
        not_ready = 4,
        ready = 8,
        no_map = 16,
        playing = 32,
        complete = 64,
        quit = 128,
        has_player = not_ready | ready | no_map | playing | complete
    }
}