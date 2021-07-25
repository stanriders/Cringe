﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Services
{
    public class ChatService
    {
        public const string LobbyName = "#lobby";

        public static readonly List<GlobalChat> GlobalChats = new()
        {
            new GlobalChat("#osu", "THIS GAME SUCKS QOQ", true),
            new GlobalChat("#announce", "Shitpost your 300pp here", true),
            new GlobalChat("#vacman", "Admin only secret chat", false, UserRanks.Admin),
            new GlobalChat("#russian", "KRYM NASH!!! :DDD"),
            new GlobalChat(LobbyName, "LOBESHNIQ")
        };

        private readonly Action<Message> _sendPrivateMessage;

        public ChatService(PlayersPool pool)
        {
            _sendPrivateMessage = message => PlayersPool.GetPlayer(message.Receiver)?.ReceiveMessage(message);
        }

        public async Task Initialize(PlayerSession player)
        {
            var rank = player.Player.UserRank;
            foreach (var globalChat in GlobalChats.Where(globalChat => IsAllowed(rank, globalChat.Accessibility)))
            {
                globalChat.ReceiveUpdates += player;
                globalChat.OnStatusUpdated();
                if (globalChat.AutoConnect)
                    await globalChat.Connect(player);
            }
        }

        public static GlobalChat GetChat(string name)
        {
            return GlobalChats.FirstOrDefault(x => x.Name == name);
        }

        public bool Purge(PlayerSession player)
        {
            var rank = player.Player.UserRank;
            foreach (var globalChat in GlobalChats.Where(globalChat => IsAllowed(rank, globalChat.Accessibility)))
                globalChat.Disconnect(player);

            return true;
        }

        public bool SendGlobalMessage(Message message)
        {
            var chat = GlobalChats.FirstOrDefault(x => x.Name == message.Receiver);

            if (chat is null) return false;
            if (!IsAllowed(message.Sender.UserRank, chat.Accessibility)) return false;

            chat.OnSendMessage(message);

            return true;
        }

        public void SendPrivateMessage(Message message)
        {
            _sendPrivateMessage(message);
        }

        /// <summary>
        ///     If player is not allowed to even see this channel 0_0
        /// </summary>
        /// <param name="userRanks">The rank of user</param>
        /// <param name="chatRanks">The accessibility level of chat</param>
        /// <returns></returns>
        private static bool IsAllowed(UserRanks userRanks, UserRanks chatRanks)
        {
            return chatRanks == UserRanks.Normal || (userRanks & chatRanks) != 0;
        }
    }
}