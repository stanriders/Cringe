using System;
using System.Threading.Tasks;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
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
                return Task.CompletedTask;
            
            if (session.MatchSession.Match.Host != session.Token.PlayerId)
                return Task.CompletedTask;
            
            session.MatchSession.Update(Match.Parse(data));
            return Task.CompletedTask;
        }
    }
}