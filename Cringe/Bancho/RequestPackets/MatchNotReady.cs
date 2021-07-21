using System;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.RequestPackets
{
    public class MatchNotReady : RequestPacket
    {
        public MatchNotReady(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchNotReady;

        public override Task Execute(UserToken token, byte[] data)
        {
            var lobby = Multiplayer.GetFromUser(token.PlayerId);
            var slot = lobby.Slots.FirstOrDefault(x => x.Player.Id == token.PlayerId);
            slot!.Status |= SlotStatus.not_ready;
            slot!.Status ^= SlotStatus.ready;
            Pool.ActionOn(lobby.Players, queue => queue.EnqueuePacket(new UpdateMatch(lobby)));
            return Task.CompletedTask;
        }
    }
}