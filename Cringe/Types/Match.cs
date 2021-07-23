using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cringe.Bancho;
using Cringe.Types.Database;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Types
{
    public class Match
    {
        public short Id { get; set; }
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
        public bool InProgress { get; set; }
        public Slot[] Slots { get; set; } = new Slot[16];

        public HashSet<int> Players { get; } = new();
        public Mods Mods { get; set; }

        public bool Connect(Player player)
        {
            var slot = Slots.OrderBy(x => x.Index).FirstOrDefault(x => x.Player is null);
            if (slot is null) return false;

            Players.Add(player.Id);
            slot.Player = player;
            slot.Status = SlotStatus.not_ready;
            return true;
        }

        public void Disconnect(Player player)
        {
            Players.Remove(player.Id);
            var slot = Slots.FirstOrDefault(x => x.Player.Id == player.Id);
            if (slot is null) return;
            slot.Mods = Mods.None;
            slot.Player = null;
            slot.Status = SlotStatus.open;
            slot.Team = MatchTeams.neutral;
        }

        public static Match Parse(byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var id = reader.ReadInt16();
            var inprogress = reader.ReadBoolean();
            reader.ReadByte();
            var mods = (Mods) reader.ReadInt32();
            var name = RequestPacket.ReadString(reader.BaseStream);
            var password = RequestPacket.ReadString(reader.BaseStream);
            var lobby = new Match
            {
                Id = id,
                Name = name,
                Password = password,
                InProgress = inprogress,
                Mods = mods,
                MapName = RequestPacket.ReadString(reader.BaseStream),
                MapId = reader.ReadInt32(),
                MapMd5 = RequestPacket.ReadString(reader.BaseStream)
            };
            for (var i = 0; i < lobby.Slots.Length; i++)
                lobby.Slots[i] = new Slot {Index = i, Status = (SlotStatus) reader.ReadByte()};

            foreach (var slot in lobby.Slots) slot.Team = (MatchTeams) reader.ReadByte();

            foreach (var slot in lobby.Slots)
                if ((byte) (slot.Status & SlotStatus.has_player) != 0) //If slot is not empty
                    reader.ReadInt32();

            lobby.Host = reader.ReadInt32();
            lobby.Mode = (GameModes) reader.ReadByte();
            lobby.WinConditions = (MatchWinConditions) reader.ReadByte();
            lobby.TeamTypes = (MatchTeamTypes) reader.ReadByte();
            lobby.FreeMode = reader.ReadByte() == 1;
            if (!lobby.FreeMode) return lobby;
            foreach (var slot in lobby.Slots)
                slot.Mods = (Mods) reader.ReadByte();

            return lobby;
        }
    }
}