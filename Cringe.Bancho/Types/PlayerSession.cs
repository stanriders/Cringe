﻿using Cringe.Bancho.Bancho;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Database;

namespace Cringe.Bancho.Types
{
    public class PlayerSession
    {
        public Player Player { get; set; }
        public UserToken Token { get; set; }
        public PacketQueue Queue { get; } = new();
        public MatchSession MatchSession { get; set; }

        #region Login / Logout triggers
        public void PlayerLoggedIn(PlayerSession player)
        {
            Queue.EnqueuePacket(new UserStats(player.GetStats()));
            Queue.EnqueuePacket(new UserPresence(player.GetPresence()));
            //TODO: Green message on friend login
        }

        public void PlayerLoggedOut(PlayerSession player)
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

        public void UpdateMatch(Match match)
        {
            Queue.EnqueuePacket(new UpdateMatch(match));
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

        #region Presence
        public Presence GetPresence()
        {
            return new()
            {
                UserId = Player.Id,
                Username = Player.Username,
                Timezone = 0x3, //Moscow UTC
                Country = 0x1F, //Brazil Code
                UserRank = Player.UserRank,
                Longitude = 0.0f,
                Latitude = 0.0f,
                GameRank = Player.Rank
            };
        }

        public Stats GetStats()
        {
            return new()
            {
                UserId = (uint) Player.Id,
                Action = new ChangeAction
                {
                    ActionId = 0,
                    ActionText = "PA3BODNT JIOXOB",
                    ActionMd5 = "",
                    ActionMods = 0,
                    GameMode = 0,
                    BeatmapId = 0
                },
                RankedScore = Player.TotalScore,
                Accuracy = Player.Accuracy,
                Playcount = Player.Playcount,
                TotalScore = Player.TotalScore,
                GameRank = Player.Rank,
                Pp = Player.Pp
            };
        }
        #endregion
    }
}
