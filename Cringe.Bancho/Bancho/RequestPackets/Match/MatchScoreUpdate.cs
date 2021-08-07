using System;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
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

            data[4] = (byte) session.MatchSession.Match.GetPlayerPosition(session.Id);
            var packet = new ResponsePackets.Match.MatchScoreUpdate(data);
            foreach (var player in session.MatchSession.Match.Players.Where(x => x.Status == SlotStatus.Playing))
                player.Player.Queue.EnqueuePacket(packet);

            return Task.CompletedTask;
        }
    }
}
