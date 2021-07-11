using System.Collections.Generic;
using Cringe.Bancho.Packets;
using Cringe.Types;
using Microsoft.Extensions.Logging;

namespace Cringe.Services
{
    public class ChatService
    {
        private readonly ILogger<ChatService> _logger;
        private readonly BanchoServicePool _pool;
        
        private readonly Dictionary<string, Chat> _chatPool;

        public ChatService(ILogger<ChatService> logger, BanchoServicePool pool)
        {
            _logger = logger;
            _pool = pool;
            _chatPool = new Dictionary<string, Chat>();
            logger.LogInformation("Creating default chats");
            var osu = new Chat("#osu", "AYE NIGGEROVIDZE");
            var announce = new Chat("#announce", "shitpost your scores here");
            var admin = new Chat("#vacman", "aye only admin chat");
            _chatPool.Add(osu.Name, osu);
            _chatPool.Add(announce.Name, announce);
            _chatPool.Add(admin.Name, admin);
        }

        public void Handle(int user, string channelName, bool disconnect = false)
        {
            var success = _chatPool.TryGetValue(channelName, out var chat);
            if(!success)
                return;
            if(disconnect)
                chat.Users.Remove(user);
            else
                chat.Users.Add(user);
            var channelInfo = new ChannelInfo(chat);
            _pool.ActionMap(queue =>
            {
                queue.EnqueuePacket(channelInfo);
            });
        }

        public void NukeUser(int user)
        {
            foreach (var chat in _chatPool)
            {
                Handle(user, chat.Key, true);
            }
        }

    }
}