using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Spectate
{
    public class StopSpectating : RequestPacket
    {
        public StopSpectating(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.StopSpectating;
        protected override string ApiPath => "spectate/status";

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.SpectateSession is null)
            {
                Logger.LogError("{Token} | Attempted to stop spectating as non-spectator", session.Token);

                return Task.CompletedTask;
            }

            session.SpectateSession.Disconnect(session);

            return Task.CompletedTask;
        }
    }
}
