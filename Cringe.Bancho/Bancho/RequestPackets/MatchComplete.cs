using System;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class MatchComplete : RequestPacket
    {
        public static object key = new();
        public MatchComplete(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type
        {
            get;
        }

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
            {
                return Task.CompletedTask;
            }

            var match = session.MatchSession.Match;
            var slot = match.GetPlayer(session.Token.PlayerId);

            lock (key) //I believe we may have problems without mutex
            {
                slot.Status = SlotStatus.complete;
                if (match.Slots.Any(x => x.Status == SlotStatus.playing))
                    return Task.CompletedTask;
            }

            match.InProgress = false;
            foreach (var player in match.Players)
            {
                if (player.Status != SlotStatus.complete) continue;

                player.Status = SlotStatus.not_ready;
                player.Player.Queue.EnqueuePacket(new ResponsePackets.MatchComplete());
            }

            session.MatchSession.OnUpdateMatch();
            return Task.CompletedTask;
        }
    }
}
