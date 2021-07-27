using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class PartMatch : RequestPacket
    {
        public PartMatch(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.PartMatch;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
            {
                Logger.LogCritical("{Token} | User leaves the match while not being in a match", session.Token);

                return Task.CompletedTask;
            }

            session.MatchSession.Disconnect(session);
            Logger.LogDebug("{Token} | User leaves a match", session.Token);

            return Task.CompletedTask;
        }
    }
}
