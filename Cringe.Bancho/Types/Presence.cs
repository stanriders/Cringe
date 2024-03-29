﻿using Cringe.Types.Enums;

namespace Cringe.Bancho.Types
{
    public class Presence
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public byte Timezone { get; set; }
        public byte Country { get; set; }
        public UserRanks UserRank { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public uint GameRank { get; set; }
    }
}
