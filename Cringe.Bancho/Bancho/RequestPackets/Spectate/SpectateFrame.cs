using System;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Spectate;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets.Spectate
{
    public class SpectateFrame : RequestPacket
    {
        public SpectateFrame(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.SpectateFrames;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            var spec = session.SpectateSession;

            if (spec.Host.Id != session.Id) return Task.CompletedTask;

            var frame = new SpectateFrames(data);
            foreach (var viewer in spec.Viewers)
                viewer.Queue.EnqueuePacket(frame);

            return Task.CompletedTask;
        }
    }
}
