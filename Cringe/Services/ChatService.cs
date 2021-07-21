using System.Collections.Generic;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types.Bancho;
using Microsoft.Extensions.Logging;

namespace Cringe.Services
{
    public class ChatServicePool
    {
        private readonly Dictionary<string, Chat> _chatPool;
        private readonly ILogger<ChatServicePool> _logger;
        private readonly BanchoServicePool _pool;

        public ChatServicePool(ILogger<ChatServicePool> logger, BanchoServicePool pool)
        {
            _logger = logger;
            _pool = pool;
            _chatPool = new Dictionary<string, Chat>();
            logger.LogInformation("Creating default chats");
            var osu = new Chat("#osu", "AYE NIGGEROVIDZE", true);
            var announce = new Chat("#announce", "shitpost your scores here", true);
            var russian = new Chat("#russian", "RUSSIAN");
            var admin = new Chat("#vacman", "aye only admin chat", true);
            _chatPool.Add(osu.Name, osu);
            _chatPool.Add(announce.Name, announce);
            _chatPool.Add(admin.Name, admin);
            _chatPool.Add(russian.Name, russian);
            _chatPool.Add(Chat.Lobby.Name, Chat.Lobby);
        }

        public HashSet<int> GetLobbyUsers()
        {
            return _chatPool[Chat.Lobby.Name].Users;
        }
        public void Create(Chat chat)
        {
            if (_chatPool.ContainsKey(chat.KvName)) return;
            _chatPool.Add(chat.KvName, chat);
        }

        public void AutoJoinOrPackInfo(int user, string channelName)
        {
            var success = _chatPool.TryGetValue(channelName, out var chat);
            if (!success)
                return;
            if (chat.AutoJoin)
            {
                chat.Users.Add(user);
                _pool.ActionMap(queue => queue.EnqueuePacket(new ChannelAutoJoin(chat)));
                _pool.ActionOn(user, queue => queue.EnqueuePacket(new ChannelJoinSuccess(chat)));
            }
            else
            {
                _pool.ActionOn(user, queue => queue.EnqueuePacket(new ChannelInfo(chat)));
            }
        }

        public void Connect(int user, string channelName)
        {
            if (!_chatPool.TryGetValue(channelName, out var chat))
                return;
            chat.Users.Add(user);
            _pool.ActionOn(user, queue => queue.EnqueuePacket(new ChannelJoinSuccess(chat)));
            _pool.ActionMap(queue => queue.EnqueuePacket(new ChannelInfo(chat)));
        }

        public void Disconnect(int user, string channelName)
        {
            if (!_chatPool.TryGetValue(channelName, out var chat))
                return;
            chat.Users.Remove(user);
            _pool.ActionOn(user, queue => queue.EnqueuePacket(new ChannelKick(chat)));
            _pool.ActionMap(queue => queue.EnqueuePacket(new ChannelInfo(chat)));
        }

        public void NukeUserFromPrivateChat(int user, string channelName)
        {
            if (!_chatPool.TryGetValue(channelName, out var chat))
                return;
            chat.Users.Remove(user);
            _pool.ActionOn(user, queue => queue.EnqueuePacket(new ChannelKick(chat)));
            if (chat.Users.Count == 0)
                _chatPool.Remove(channelName);
        }

        public void NukeUser(int user)
        {
            foreach (var chat in _chatPool) Disconnect(user, chat.Key);
        }
    }
}