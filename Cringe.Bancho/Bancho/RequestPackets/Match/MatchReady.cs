using System;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
{
    public class MatchReady : RequestPacket
    {
        public MatchReady(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchReady;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            var slot = session.MatchSession?.Match.Slots.FirstOrDefault(x => x.Player == session);
            if (slot is null)
            {
                Logger.LogError(
                    "{Token} | User tries to set ready status while his MatchSession is null or user hasn't been founded in slots. Match info: {@Session}",
                    session.Token, session.MatchSession);
                session.UpdateMatch(Types.Match.NullMatch);

                return Task.CompletedTask;
            }

            slot.Status = SlotStatus.Ready;
            session.MatchSession.OnUpdateMatch(true);

            return Task.CompletedTask;
        }
    }
}
