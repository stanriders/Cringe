using System;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types.Database;
using Cringe.Types.Enums;
using Cringe.Types.OsuApi;

namespace Cringe.Types
{
    public class GlobalChat : ISocial
    {
        public string Name { get; }
        public string Description { get; }
        public UserRanks Accessibility { get; }
        public int Count { get; private set; }
        public bool AutoConnect { get; }

        public event Action<Message> SendMessage;
        public event Action<GlobalChat> StatusUpdated; 
        public GlobalChat(string name, string description, bool autoConnect = false, UserRanks accessibility = UserRanks.Normal)
        {
            Name = name;
            Description = description;
            AutoConnect = autoConnect;
            Accessibility = accessibility;
        }

        public Task<bool> Connect(PlayerSession player)
        {
            SendMessage += player.ReceiveMessage;
            Count++;
            if(AutoConnect)
                player.ChatAutoJoin(this);
            player.ChatConnected(this);
            OnStatusUpdated(this);
            return Task.FromResult(true);
        }

        public bool Disconnect(PlayerSession player)
        {
            SendMessage -= player.ReceiveMessage;
            Count--;
            player.ChatKick(this);
            OnStatusUpdated(this);
            return true;
        }

        public void OnSendMessage(Player sender, string content)
        {
            OnSendMessage(new Message(content, sender, Name));
        }
        public virtual void OnSendMessage(Message obj)
        {
            SendMessage?.Invoke(obj);
        }

        protected virtual void OnStatusUpdated(GlobalChat obj)
        {
            StatusUpdated?.Invoke(obj);
        }
    }
}