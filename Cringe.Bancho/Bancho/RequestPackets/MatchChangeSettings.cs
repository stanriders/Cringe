using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class MatchChangeSettings : RequestPacket
    {
        public MatchChangeSettings(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchChangeSettings;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
            {
                Logger.LogError("{Token} | User tries to change match's settings while his MatchSession is null", session.Token);
                return Task.CompletedTask;
            }

            if (session.MatchSession.Match.Host != session.Token.PlayerId)
            {
                Logger.LogInformation("{Token} | User tries to change match's settings while not being a host", session.Token);
                return Task.CompletedTask;
            }

            var match = Match.Parse(data);
            session.MatchSession.Update(match);
            Logger.LogDebug("{Token} | User changes the settings of the match to {@Match}", session.Token, match);

            return Task.CompletedTask;
        }
    }
}
