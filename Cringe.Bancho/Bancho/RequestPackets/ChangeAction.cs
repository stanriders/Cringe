using System;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class ChangeAction : RequestPacket
    {
        public ChangeAction(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.ChangeAction;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            var action = Types.ChangeAction.Parse(data);
            var stats = session.GetStats();
            stats.Action = action;

            Logger.LogInformation("{Token} | Changes action to {@Action}", session.Token, stats.Action);

            Stats.SetUpdates(session.Token.PlayerId, stats);
            session.Queue.EnqueuePacket(new UserStats(stats));
            session.Queue.EnqueuePacket(new UserPresence(session.GetPresence()));

            return Task.CompletedTask;
        }
    }
}
