using System;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Bancho;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class ChangeActionPacket : RequestPacket
    {
        public ChangeActionPacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.ChangeAction;

        public override async Task Execute(UserToken token, byte[] data)
        {
            var action = ChangeAction.Parse(data);
            var player = await Token.GetPlayerWithoutScores(token.PlayerId);
            var pl = player.Stats;
            pl.Action = action;
            Stats.SetUpdates(token.PlayerId, pl);
            var pool = Pool;
            pool.ActionOn(token.PlayerId, queue => queue.EnqueuePacket(new UserStats(pl)));
            pool.ActionOn(token.PlayerId, queue => queue.EnqueuePacket(new UserPresence(player.Presence)));
        }
    }
}