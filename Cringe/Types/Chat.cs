using System.Collections.Generic;

namespace Cringe.Types
{
    public class Chat
    {

        public Chat(string name, string description)
        {
            Name = name;
            Description = description;
            Users = new HashSet<int>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public HashSet<int> Users { get; }
    }
}