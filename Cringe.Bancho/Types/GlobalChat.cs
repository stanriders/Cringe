using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Types
{
    public class GlobalChat
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
        public static GlobalChat Spectate => new("#spectate", "niga");

        public static GlobalChat SpectateCount(int count)
        {
            var spec = Spectate;
            spec.Count = count;

            return spec;
        }

        public string Name { get; }
        public string Description { get; }
        public UserRanks Accessibility { get; }
        public int Count { get; private set; }
        public bool AutoConnect { get; }

        public void Connect(PlayerSession player)
        {
            SendMessage += player;
            Count = SendMessage.Count;

            if (AutoConnect)
                player.ChatAutoJoin(this);

            player.ChatConnected(this);
            OnStatusUpdated();
        }

        public void Disconnect(PlayerSession player)
        {
            SendMessage -= player;
            Count = SendMessage.Count;

            player.ChatKick(this);
            OnStatusUpdated();
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
