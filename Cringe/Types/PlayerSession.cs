using Cringe.Bancho;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types.Database;

namespace Cringe.Types
{
    public class PlayerSession
    {
        public Player Player { get; set; }
        public UserToken Token { get; set; }
        public PacketQueue Queue { get; } = new();

        #region Login / Logout triggers

        public void PlayerLoggedIn(Player player)
        {
            Queue.EnqueuePacket(new UserStats(player.Stats));
            Queue.EnqueuePacket(new UserPresence(player.Presence));
            //TODO: Green message on friend login
        }

        public void PlayerLoggedOut(Player player)
        {
            //TODO: Red message on friend logout
        }

        #endregion

        #region Lobby

        public void NewMatch(Match match)
        {
            Queue.EnqueuePacket(new NewMatch(match));
        }

        public void DisposeMatch(Match match)
        {
            Queue.EnqueuePacket(new DisposeMatch(match));
        }

        #endregion

        #region Messages and Chats

        public void ChatInfo(GlobalChat chat)
        {
            Queue.EnqueuePacket(new ChannelInfo(chat));
        }

        public void ChatConnected(GlobalChat chat)
        {
            Queue.EnqueuePacket(new ChannelJoinSuccess(chat));
        }

        public void ChatKick(GlobalChat chat)
        {
            Queue.EnqueuePacket(new ChannelKick(chat));
        }

        public void ChatAutoJoin(GlobalChat chat)
        {
            Queue.EnqueuePacket(new ChannelAutoJoin(chat));
        }

        public void ReceiveMessage(Message message)
        {
            if (message.Sender.Username != Token.Username)
                Queue.EnqueuePacket(message);
        }

        #endregion
    }
}