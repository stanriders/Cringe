using System;
using System.Threading.Tasks;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
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
                session.MatchSession = null;
                throw new Exception($"{session.Player.Id} tries to leave the match while not being assigned to it");
            }

            session.MatchSession.Disconnect(session);
            return Task.CompletedTask;
        }
    }
}