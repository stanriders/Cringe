using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class MatchHasBeatmap : RequestPacket
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

            var slot = session.MatchSession.Match.GetPlayer(session.Token.PlayerId);
            slot.Status = SlotStatus.not_ready;
            session.MatchSession.OnUpdateMatch();

            return Task.CompletedTask;
        }
    }
}
