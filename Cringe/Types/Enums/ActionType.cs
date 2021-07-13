namespace Cringe.Types.Enums
{
    public enum ActionType : byte
    {
        Idle = 0x00,
        Afk = 0x01,
        Playing = 0x02,
        Editing = 0x03,
        Modding = 0x04,
        Multiplayer = 0x05,
        Watching = 0x06,
        Unknown = 0x07,
        Testing = 0x08,
        Submitting = 0x09,
        Paused = 0x0A,
        Lobby = 0x0B,
        Multiplaying = 0x0C,
        OsuDirect = 0x0D
    }
}