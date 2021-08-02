using System;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Spectate;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

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

            if (spec is null)
            {
                Logger.LogError("{Token} | SpectateSession is null and SpectateFrame is invoked", session.Token);
                return Task.CompletedTask;
            }

            if (spec.Host.Id != session.Id) return Task.CompletedTask;

            var frame = new SpectateFrames(data);
            foreach (var viewer in spec.Viewers.Values)
                viewer.Queue.EnqueuePacket(frame);

            return Task.CompletedTask;
        }
    }
}
