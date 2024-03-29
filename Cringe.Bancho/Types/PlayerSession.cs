﻿using System;
using System.Text.Json.Serialization;
using Cringe.Bancho.Bancho;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Bancho.ResponsePackets.Match;
using Cringe.Types;
using Cringe.Types.Database;
using Destructurama.Attributed;

namespace Cringe.Bancho.Types
{
    public class PlayerSession
    {
        [JsonIgnore]
        [NotLogged]
        public int Id => Player.Id;
        public Player Player { get; set; }

        public DateTime LastUpdate { get; set; }

        [JsonIgnore]
        [NotLogged]
        public UserToken Token { get; set; }

        [JsonIgnore]
        [NotLogged]
        public PacketQueue Queue { get; } = new();

        [JsonIgnore]
        [NotLogged]
        public MatchSession MatchSession { get; set; }

        [JsonIgnore]
        [NotLogged]
        public SpectateSession SpectateSession { get; set; }

        #region Login / Logout triggers
        public void PlayerLoggedIn(PlayerSession player)
        {
            Queue.EnqueuePacket(new UserStats(player.Stats));
            Queue.EnqueuePacket(new UserPresence(player.Presence));
        }

        public void PlayerLoggedOut(PlayerSession player)
        {
            Queue.EnqueuePacket(new UserLogout(player.Player.Id));
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

        public void ReceiveNotification(string notification)
        {
            Queue.EnqueuePacket(new Notification(notification));
        }
        #endregion

        #region Presence
        public Presence Presence => new()
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

        public Stats Stats => new()
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

        public void UpdateStats(Stats stats)
        {
            Queue.EnqueuePacket(new UserStats(stats));
            Queue.EnqueuePacket(new UserPresence(Presence));
        }
        #endregion
    }
}
