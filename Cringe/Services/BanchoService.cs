using System;
using System.Collections.Generic;
using System.IO;
using Cringe.Bancho;

namespace Cringe.Services
{
    public class BanchoService
    {
        private readonly Queue<byte[]> queue = new();

        public byte[] GetDataToSend()
        {
            using var stream = new MemoryStream();
            while (queue.TryDequeue(out var data) && stream.Length < 10000000)
                stream.Write(data);

            return stream.ToArray();
        }

        public void EnqueuePacket(DataPacket packet)
        {
            using var stream = new MemoryStream();

            var data = packet.GetBytes();

            stream.Write(BitConverter.GetBytes((short) packet.Type));
            stream.WriteByte(0);
            stream.Write(BitConverter.GetBytes(data.Length));
            stream.Write(data);

            queue.Enqueue(stream.ToArray());
        }
    }
}