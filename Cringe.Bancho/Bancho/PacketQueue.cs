using System;
using System.Collections.Generic;
using System.IO;
using Cringe.Bancho.Bancho.ResponsePackets;
using Microsoft.AspNetCore.Mvc;

namespace Cringe.Bancho.Bancho
{
    public class PacketQueue
    {
        private readonly Queue<byte[]> queue = new();

        public byte[] GetDataToSend()
        {
            using var stream = new MemoryStream();
            while (queue.TryDequeue(out var data) && stream.Length < 10000000)
                stream.Write(data);

            return stream.ToArray();
        }

        public static PacketQueue NoPacket()
        {
            return new();
        }

        public static PacketQueue NullUser()
        {
            return new PacketQueue().EnqueuePacket(new UserId(-1));
        }

        public PacketQueue EnqueuePacket(ResponsePacket packet)
        {
            using var stream = new MemoryStream();

            var data = packet.GetBytes();

            stream.Write(BitConverter.GetBytes((short) packet.Type));
            stream.WriteByte(0);
            stream.Write(BitConverter.GetBytes(data.Length));
            stream.Write(data);

            queue.Enqueue(stream.ToArray());

            return this;
        }

        public FileContentResult GetResult()
        {
            return new(GetDataToSend(), "text/html; charset=UTF-8");
        }
    }
}
