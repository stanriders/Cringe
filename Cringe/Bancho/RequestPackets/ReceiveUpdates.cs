using System;
using System.Threading.Tasks;
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
            return Task.CompletedTask;
        }
    }
}