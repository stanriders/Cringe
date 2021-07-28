using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Types.Database;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.ResponsePackets
{
    public class Message : ResponsePacket
    {
        public Message(string message, Player who, string receiver)
        {
            Content = message;
            Sender = who;
            Receiver = receiver;
        }

        public string Content { get; }
        public Player Sender { get; set; }
        public string Receiver { get; }

        public override ServerPacketType Type => ServerPacketType.SendMessage;

        public override byte[] GetBytes()
        {
            var data = PackData(Sender.Username).AsEnumerable();
            data = data.Concat(PackData(Content));
            data = data.Concat(PackData(Receiver));
            data = data.Concat(PackData(Sender.Id));

            return data.ToArray();
        }

        public static async Task<Message> Parse(byte[] data, string username)
        {
            await using var stream = new MemoryStream(data);
            var text = RequestPacket.ReadString(stream);
            var receiver = RequestPacket.ReadString(stream);

            return new Message(text, new Player {Username = username, UserRank = UserRanks.Normal}, receiver);
        }

        public override string ToString()
        {
            return $"{Sender.Id}|{Receiver}|{Content}";
        }
    }
}
