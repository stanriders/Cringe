using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
{
    public class MatchNoBeatmap : MatchChanged
    {
        public MatchNoBeatmap(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchNoBeatmap;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
            {
                Logger.LogError("{Token} | No beatmap status while not in match", session.Token);

                return Task.CompletedTask;
            }

            var match = session.MatchSession.Match;
            var slot = match.GetPlayer(session.Id);
            slot.Status = SlotStatus.NoMap;
            session.MatchSession.OnUpdateMatch();

            return Task.CompletedTask;
        }
    }
}
