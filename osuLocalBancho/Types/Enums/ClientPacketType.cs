namespace osuLocalBancho.Types.Enums
{
    public enum ClientPacketType : ushort
    {
        ChangeAction = 0,
        RequestStatusUpdate = 3,
        Ping = 4,
        UserStatsRequest = 85,
        UserPanelRequest = 97
    }
}
