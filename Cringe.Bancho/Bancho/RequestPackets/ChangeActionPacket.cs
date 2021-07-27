using System;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

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
            var stats = session.GetStats();
            stats.Action = action;

            Logger.LogInformation("{Token} | Changes stats to {@Stats}", session.Token, stats);

            Stats.SetUpdates(session.Token.PlayerId, stats);
            session.Queue.EnqueuePacket(new UserStats(stats));
            session.Queue.EnqueuePacket(new UserPresence(session.GetPresence()));

            return Task.CompletedTask;
        }
    }
}
