using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Services
{
    public class ChatService
    {
        private readonly Action<Message> _sendPrivateMessage;
        public ChatService(PlayersPool pool)
        {
            _sendPrivateMessage = message => pool.GetPlayer(message.Receiver)?.ReceiveMessage(message);
        }

        public const string LobbyName = "#lobby";
        public static readonly List<GlobalChat> GlobalChats = new()
        {
            new GlobalChat("#osu", "THIS GAME SUCKS QOQ", true),
            new GlobalChat("#announce", "Shitpost your 300pp here", true),
            new GlobalChat("#vacman", "Admin only secret chat"),
            new GlobalChat(LobbyName, "LOBESHNIQ")
        };

        public async Task Initialize(PlayerSession player)
        {
            var rank = player.Player.UserRank;
            foreach (var globalChat in GlobalChats.Where(globalChat => IsAllowed(rank, globalChat.Accessibility)))
            {
                globalChat.StatusUpdated += player.ChatInfo;
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