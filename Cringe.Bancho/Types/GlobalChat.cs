using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Types.Database;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Types
{
    public class GlobalChat : ISocial
    {
        public PlayerEvent ReceiveUpdates = new();
        public PlayerEvent SendMessage = new();

        public GlobalChat(string name, string description, bool autoConnect = false,
            UserRanks accessibility = UserRanks.Normal)
        {
            Name = name;
            Description = description;
            AutoConnect = autoConnect;
            Accessibility = accessibility;
        }

        public static GlobalChat Multiplayer => new("#multiplayer", "/a/");

        public string Name { get; }
        public string Description { get; }
        public UserRanks Accessibility { get; }
        public int Count { get; private set; }
        public bool AutoConnect { get; }

        public Task<bool> Connect(PlayerSession player)
        {
            SendMessage += player;
            Count = SendMessage.Count;

            if (AutoConnect)
                player.ChatAutoJoin(this);

            player.ChatConnected(this);
            OnStatusUpdated();

            return Task.FromResult(true);
        }

        public bool Disconnect(PlayerSession player)
        {
            SendMessage -= player;
            Count = SendMessage.Count;

            player.ChatKick(this);
            OnStatusUpdated();

            return true;
        }


        public void OnSendMessage(Player sender, string content)
        {
            OnSendMessage(new Message(content, sender, Name));
        }

        public virtual void OnSendMessage(Message obj)
        {
            SendMessage.Invoke(x => x.ReceiveMessage(obj));
        }

        public virtual void OnStatusUpdated()
        {
            ReceiveUpdates.Invoke(x => x.ChatInfo(this));
        }
    }
}
