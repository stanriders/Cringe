using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
{
    public class CreateMatch : RequestPacket
    {
        public CreateMatch(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.CreateMatch;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            session.MatchSession = null;
            var match = Types.Match.Parse(data);
            var matchSession = Lobby.CreateMatch(session, match);

            if (session.MatchSession is null)
            {
                Logger.LogCritical(
                    "{Token} | the host's matchSession after creating a match is null\nShould be: {Match}",
                    session.Token, matchSession.Match);

                throw new Exception("The host's matchSession after creating a match is null");
            }

            Logger.LogInformation("{Token} | Created a match ({@Match})", session.Token, session.MatchSession.Match);

            return Task.CompletedTask;
        }
    }
}
