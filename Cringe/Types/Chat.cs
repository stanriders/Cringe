using System.Collections.Generic;

namespace Cringe.Types
{
    public class Chat
    {
        public Chat(string name, string description, bool autoJoin = false)
        {
            Name = name;
            Description = description;
            AutoJoin = autoJoin;
            Users = new HashSet<int>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool AutoJoin { get; }
        public HashSet<int> Users { get; }

        public static Chat Lobby = new Chat("#lobby", "JUGAMA MULTIPLAYER");
    }
}