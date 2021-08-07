using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Cringe.Bancho.Bancho;
using Cringe.Types;
using Cringe.Types.Database;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Types
{
    public class Match : MatchModel
    {
        public override Player[] Players => OccupiedSlots.Select(x => x.Player.Player).ToArray();

        [JsonIgnore]
        public List<Slot> Slots { get; set; } = new int[16].Select(_ => new Slot()).ToList();

        [JsonIgnore]
        public IEnumerable<Slot> OccupiedSlots => Slots.Where(x => x.Player is not null);

        [JsonIgnore]
        public IEnumerable<Slot> PlayingPlayers => OccupiedSlots.Where(x => x.Status == SlotStatus.Playing);

        public static Match NullMatch => new()
        {
            Id = 0,
            Name = "",
            Password = "",
            Host = 0,
            MapId = 0,
            MapMd5 = "",
            MapName = "",
            Mode = GameModes.Osu,
            FreeMode = false,
            InProgress = false,
            Mods = Mods.None
        };

        public Slot GetHost()
        {
            return GetPlayer(Host);
        }

        public Slot GetPlayer(int id)
        {
            return Slots.FirstOrDefault(x => x.Player?.Id == id);
        }

        public int GetPlayerPosition(int id)
        {
            return Slots.FindIndex(x => x.Player?.Id == id);
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
            for (var i = 0; i < 16; i++)
                lobby.Slots[i] = new Slot {Status = (SlotStatus) reader.ReadByte()};

            foreach (var slot in lobby.Slots) slot.Team = (MatchTeams) reader.ReadByte();

            foreach (var slot in lobby.Slots.Where(slot => (byte) (slot.Status & SlotStatus.HasPlayer) != 0))
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

        public override string ToString()
        {
            return
                $"{Id}|{Name}|{Password}|{Host}|{MapId}|{MapName}|{MapMd5}|{Mode}|{FreeMode}|{WinConditions}|{TeamTypes}|{InProgress}|{string.Join(",", Slots)}|{Mods}";
        }
    }
}
