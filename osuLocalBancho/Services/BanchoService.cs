using System.Collections.Generic;
using System.IO;
using osuLocalBancho.Bancho;
using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Services
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

        public void EnqueuePacket(byte[] data, ServerPacketType type)
        {
            using var stream = new MemoryStream();

            stream.Write(DataPacket.PackData((short)type));
            stream.WriteByte(0);
            stream.Write(DataPacket.PackData(data.Length));
            stream.Write(data);

            queue.Enqueue(stream.ToArray());
        }
    }
}
