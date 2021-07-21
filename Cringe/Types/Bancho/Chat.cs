using System.Collections.Generic;

namespace Cringe.Types.Bancho
{
    public class Chat
    {
        public static Chat Lobby = new("#lobby", "JUGAMA MULTIPLAYER");

        public Chat(string name, string description, bool autoJoin = false)
        {
            Name = name;
            Description = description;
            AutoJoin = autoJoin;
            Users = new HashSet<int>();
        }

        public string Name { get; set; }
        public string UniqueId { get; set; }
        public string KvName => Name + UniqueId;
        public string Description { get; set; }
        public bool AutoJoin { get; }
        public HashSet<int> Users { get; }

        public static Chat Multiplayer(string uniqueId)
        {
            return new("#multiplayer", "/a/")
            {
                UniqueId = uniqueId,
            };
        }
    }
}