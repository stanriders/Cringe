using System;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class RequestStatusUpdate : RequestPacket
    {
        public RequestStatusUpdate(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.RequestStatusUpdate;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            session.Queue.EnqueuePacket(new UserStats(session.Player.Stats));
            return Task.CompletedTask;
        }
    }
}