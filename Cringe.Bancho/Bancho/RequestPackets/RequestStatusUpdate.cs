using System;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class RequestStatusUpdate : RequestPacket
    {
        public RequestStatusUpdate(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.RequestStatusUpdate;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            session.Queue.EnqueuePacket(new UserStats(session.GetStats()));
            return Task.CompletedTask;
        }
    }
}