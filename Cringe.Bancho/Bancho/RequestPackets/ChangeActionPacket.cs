using System;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class ChangeActionPacket : RequestPacket
    {
        public ChangeActionPacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.ChangeAction;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            var action = ChangeAction.Parse(data);
            var pl = session.GetStats();
            pl.Action = action;
            Stats.SetUpdates(session.Token.PlayerId, pl);
            session.Queue.EnqueuePacket(new UserStats(pl));
            session.Queue.EnqueuePacket(new UserPresence(session.GetPresence()));

            return Task.CompletedTask;
        }
    }
}
