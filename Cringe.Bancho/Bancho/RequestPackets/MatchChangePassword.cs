using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class MatchChangePassword : RequestPacket
    {
        public MatchChangePassword(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchChangePassword;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
            {
                return Task.CompletedTask;
            }

            if (session.MatchSession.Match.Host != session.Id)
            {
                Logger.LogError("{Token} | Attempted to change password as non-host. Match info: {@Match}", session.Token, session.MatchSession.Match);
                return Task.CompletedTask;
            }

            var match = Match.Parse(data);
            session.MatchSession.Match.Password = match.Password;
            session.MatchSession.OnUpdateMatch(true);
            return Task.CompletedTask;
        }
    }
}
