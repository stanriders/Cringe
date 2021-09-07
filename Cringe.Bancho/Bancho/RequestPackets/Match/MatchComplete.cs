using System;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
{
    public class MatchComplete : MatchChanged
    {
        public static object key = new();

        public MatchComplete(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchComplete;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
                return Task.CompletedTask;

            var match = session.MatchSession.Match;
            var slot = match.GetPlayer(session.Id);

            lock (key) //I believe we may have problems without mutex
            {
                slot.Status = SlotStatus.Complete;

                if (match.Slots.Any(x => x.Status == SlotStatus.Playing))
                    return Task.CompletedTask;
            }

            match.InProgress = false;
            foreach (var player in match.OccupiedSlots)
            {
                if (player.Status != SlotStatus.Complete) continue;

                player.Loaded = false;
                player.Skipped = false;
                player.Status = SlotStatus.NotReady;
                player.Player.Queue.EnqueuePacket(new ResponsePackets.Match.MatchComplete());
            }

            session.MatchSession.OnUpdateMatch();

            return Task.CompletedTask;
        }
    }
}
