using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
{
    public class MatchHasBeatmap : MatchChanged
    {
        public MatchHasBeatmap(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchHasBeatmap;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
            {
                Logger.LogError("{Token} | Has beatmap status while not in match", session.Token);

                return Task.CompletedTask;
            }

            var slot = session.MatchSession.Match.GetPlayer(session.Id);
            slot.Status = SlotStatus.NotReady;
            session.MatchSession.OnUpdateMatch();

            return Task.CompletedTask;
        }
    }
}
