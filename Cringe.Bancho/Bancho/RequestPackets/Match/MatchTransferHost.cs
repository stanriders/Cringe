using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
{
    public class MatchTransferHost : RequestPacket
    {
        public MatchTransferHost(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchTransferHost;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var slotId = reader.ReadInt32();

            if (slotId is < 0 or > 16)
                return Task.CompletedTask;

            var slot = session.MatchSession?.Match.Slots[slotId];

            if (slot?.Player is null)
                return Task.CompletedTask;

            session.MatchSession.Match.Host = slot.Player.Id;
            slot.Player.Queue.EnqueuePacket(new ResponsePackets.Match.MatchTransferHost());
            session.MatchSession.OnUpdateMatch(true);

            return Task.CompletedTask;
        }
    }
}
