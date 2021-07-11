using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class Message : DataPacket
    {
        private readonly string _message;
        private readonly string _who;

        private Message(string message, string who, string receiver)
        {
            _message = message;
            _who = who;
            Receiver = receiver;
        }

        public string Receiver { get; }

        public override ServerPacketType Type => ServerPacketType.SendMessage;

        public override byte[] GetBytes()
        {
            var data = PackData(_who).AsEnumerable();
            data = data.Concat(PackData(_message));
            data = data.Concat(PackData(Receiver));
            return data.ToArray();
        }

        public static async Task<Message> Parse(byte[] data, string username)
        {
            await using var stream = new MemoryStream(data);
            var text = ReadString(stream);
            var receiver = ReadString(stream);
            return new Message(text, username, receiver);
        }
    }
}