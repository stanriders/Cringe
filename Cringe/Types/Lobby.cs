using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cringe.Bancho;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Types
{
    public class Lobby
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int Host { get; set; }
        public int MapId { get; set; }
        public string MapMd5 { get; set; }
        public string MapName { get; set; }
        public GameModes Mode { get; set; } = GameModes.Osu;
        public bool FreeMode { get; set; }
        public MatchWinConditions WinConditions { get; set; }
        public MatchTeamTypes TeamTypes { get; set; } = MatchTeamTypes.head_to_head;
        public bool InProgress { get; set; } = false;
        public Slot[] Slots { get; set; } = new Slot[16];

        public HashSet<Player> Players { get; } = new();

        public bool Connect(Player player)
        {
            var slot = Slots.FirstOrDefault(x => x.Player is null);
            if(slot is null) return false;
            
            Players.Add(player);
            slot.Player = player;
            slot.Status = SlotStatus.not_ready;
            return true;
        }

        public void Disconnect(Player player)
        {
            Players.Remove(player);
        }

        public bool Contains(string player)
        {
            return Players.Any(x => x.Username == player);
        }

        public static Lobby Parse(byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var id = reader.ReadInt16();
            reader.ReadBytes(6);
            var name = RequestPacket.ReadString(reader.BaseStream);
            var password = RequestPacket.ReadString(reader.BaseStream);
            var lobby = new Lobby
            {
                Id = id,
                Name = name,
                Password = password,
                MapName = RequestPacket.ReadString(reader.BaseStream),
                MapId = reader.ReadInt32(),
                MapMd5 = RequestPacket.ReadString(reader.BaseStream)
            };
            for (var i = 0; i < lobby.Slots.Length; i++)
                lobby.Slots[i] = new Slot {Status = (SlotStatus) reader.ReadByte()};

            foreach (var slot in lobby.Slots) slot.Team = (MatchTeams) reader.ReadByte();

            foreach (var slot in lobby.Slots)
                if (slot.Status.HasFlag(SlotStatus.has_player))
                    reader.ReadInt32();

            var host = reader.ReadInt32();
            lobby.Host = host;
            lobby.Mode = (GameModes) reader.ReadByte();
            lobby.WinConditions = (MatchWinConditions) reader.ReadByte();
            lobby.FreeMode = reader.ReadByte() == 1;
            if (!lobby.FreeMode) return lobby;
            foreach (var slot in lobby.Slots)
                slot.Mods = (Mods) reader.ReadByte();

            return lobby;
        }
    }
}