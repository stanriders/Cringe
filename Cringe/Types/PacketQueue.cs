using System;
using System.Collections.Generic;
using System.IO;
using Cringe.Bancho;
using Cringe.Bancho.Packets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration;

namespace Cringe.Types
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

        public static FileContentResult NullUser()
        {
            return new PacketQueue().EnqueuePacket(new UserId(-1)).GetResult();
        }

        public PacketQueue EnqueuePacket(DataPacket packet)
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
            return new FileContentResult(GetDataToSend(), "text/html; charset=UTF-8");
        }
    }
}