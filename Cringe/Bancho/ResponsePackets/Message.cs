﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Types.Enums;

namespace Cringe.Bancho.ResponsePackets
{
    public class Message : ResponsePacket
    {
        private Message(string message, string who, string receiver)
        {
            Content = message;
            Sender = who;
            Receiver = receiver;
        }

        public string Content { get; }
        public string Sender { get; }

        public string Receiver { get; }

        public override ServerPacketType Type => ServerPacketType.SendMessage;

        public override byte[] GetBytes()
        {
            var data = PackData(Sender).AsEnumerable();
            data = data.Concat(PackData(Content));
            data = data.Concat(PackData(Receiver));
            return data.ToArray();
        }

        public static async Task<Message> Parse(byte[] data, string username)
        {
            await using var stream = new MemoryStream(data);
            var text = RequestPacket.ReadString(stream);
            var receiver = RequestPacket.ReadString(stream);
            return new Message(text, username, receiver);
        }
    }
}