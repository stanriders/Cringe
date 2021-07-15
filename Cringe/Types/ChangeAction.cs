using System.IO;
using Cringe.Bancho;
using Cringe.Types.Enums;

namespace Cringe.Types
{
    public class ChangeAction
    {
        public ActionType Action
        {
            get => (ActionType) ActionId;
            set => ActionId = (byte) value;
        }

        public byte ActionId { get; set; }
        public string ActionText { get; set; }
        public string ActionMd5 { get; set; }
        public uint ActionMods { get; set; }
        public byte GameMode { get; set; }
        public int BeatmapId { get; set; }

        public static ChangeAction Parse(byte[] bytes)
        {
            var reader = new BinaryReader(new MemoryStream(bytes));
            var action = new ChangeAction();
            reader.ReadBytes(7);
            action.ActionId = reader.ReadByte();
            action.ActionText = RequestPacket.ReadString(reader.BaseStream);
            action.ActionMd5 = RequestPacket.ReadString(reader.BaseStream);
            action.ActionMods = reader.ReadUInt32();
            action.GameMode = reader.ReadByte();
            action.BeatmapId = reader.ReadInt32();
            return action;
        }
    }
}