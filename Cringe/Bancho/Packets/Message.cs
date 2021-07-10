using System.Linq;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Packets
{
    public class Message : DataPacket
    {
        private readonly string _message;
        private readonly string _where;
        private readonly string _who;

        public Message(string message, string who, string where)
        {
            _message = message;
            _who = who;
            _where = where;
        }

        public override ServerPacketType Type => ServerPacketType.SendMessage;

        public override byte[] GetBytes()
        {
            var data = PackData(_who).AsEnumerable();
            data = data.Concat(PackData(_message));
            data = data.Concat(PackData(_where));
            return data.ToArray();
        }
    }
}