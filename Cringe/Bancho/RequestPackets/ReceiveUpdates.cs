using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class ReceiveUpdates : RequestPacket
    {
        public ReceiveUpdates(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.ReceiveUpdates;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var id = reader.ReadInt32();
            var player = Pool.GetPlayer(id);
            session.Queue.EnqueuePacket(new UserStats(player.Player.Stats));
            session.Queue.EnqueuePacket(new UserPresence(player.Player.Presence));
            return Task.CompletedTask;
        }
    }
}