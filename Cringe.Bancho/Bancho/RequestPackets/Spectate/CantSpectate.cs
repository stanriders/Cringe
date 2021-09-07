using System;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Spectate;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Spectate
{
    public class CantSpectate : RequestPacket
    {
        public CantSpectate(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.CantSpectate;
        protected override string ApiPath => "spectate/unable";

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.SpectateSession is null)
            {
                Logger.LogError("{Token} | Can't spectate... foryle", session.Token);
                return Task.CompletedTask;
            }

            var packet = new SpectatorCantSpectate(session.Id);
            session.SpectateSession.Host.Queue.EnqueuePacket(packet);

            foreach (var viewer in session.SpectateSession.Viewers.Values)
            {
                viewer.Queue.EnqueuePacket(packet);
            }

            return Task.CompletedTask;
        }
    }
}
