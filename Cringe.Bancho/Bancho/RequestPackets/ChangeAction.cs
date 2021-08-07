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

        public override async Task Execute(PlayerSession session, byte[] data)
        {
            var action = Types.ChangeAction.Parse(data);
            var stats = await Stats.GetUpdates(session.Id);
            stats.Action = action;

            Logger.LogInformation("{Token} | Changes action to {@Action}", session.Token, stats.Action);

            Stats.SetUpdates(session.Id, stats);
            session.Queue.EnqueuePacket(new UserStats(stats));
            session.Queue.EnqueuePacket(new UserPresence(session.Presence));
        }
    }
}
