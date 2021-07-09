namespace Cringe.Types.Enums
{
    public enum ServerPacketType : short
    {
        UserId = 5,
        UserStats = 11,
        Notification = 24,
        Supporter = 71,
        FriendsList = 72,
        ProtocolVersion = 75,
        MainMenuIcon = 76,
        UserPanel = 83,
        ChannelInfoEnd = 89,
        SilenceEnd = 92
    }
}