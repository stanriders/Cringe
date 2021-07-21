using System;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class MatchChangeSlot : RequestPacket
    {
        public MatchChangeSlot(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchChangeSlot;

        public override Task Execute(UserToken token, byte[] data)
        {
            var id = ReadInt(data);
            var lobby = Multiplayer.GetFromUser(token.PlayerId);
            if (lobby is null) throw new Exception("Null lobby");

            var slot = lobby.Slots.FirstOrDefault(x => x.Player.Id == token.PlayerId);
            var slotPos = lobby.Slots.FirstOrDefault(x => x.Index == id);
            if (slotPos is null) throw new Exception("No slotPos found");

            slotPos.Index = slot!.Index;
            slot!.Index = id;
            Pool.ActionOn(lobby.Players.Select(x => x.Id), queue => queue.EnqueuePacket(new UpdateMatch(lobby)));
            return Task.CompletedTask;
        }
    }
}