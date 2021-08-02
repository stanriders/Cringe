using System;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class MatchLoadComplete : RequestPacket
    {
        private readonly object key = new();

        public MatchLoadComplete(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchLoadComplete;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
                return Task.CompletedTask;

            var match = session.MatchSession.Match;

            lock (key)
            {
                var slot = match.GetPlayer(session.Id);
                slot.Loaded = true;

                if (match.Players.Where(x => x.Status == SlotStatus.playing).Any(player => !player.Loaded))
                    return Task.CompletedTask;
            }

            foreach (var player in match.Players)
                player.Player.Queue.EnqueuePacket(new ResponsePackets.MatchLoadComplete());

            return Task.CompletedTask;
        }
    }
}
