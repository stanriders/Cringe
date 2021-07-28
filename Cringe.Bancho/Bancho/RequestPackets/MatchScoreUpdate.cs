using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class MatchScoreUpdate : RequestPacket
    {
        public MatchScoreUpdate(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchScoreUpdate;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
                return Task.CompletedTask;

            foreach (var player in session.MatchSession.Match.Players)
            {
                player.Player.Queue.EnqueuePacket(new ResponsePackets.MatchScoreUpdate(data));
            }

            return Task.CompletedTask;
        }
    }
}
