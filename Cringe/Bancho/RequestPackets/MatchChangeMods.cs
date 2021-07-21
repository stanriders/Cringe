using System;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class MatchChangeMods : RequestPacket
    {
        public MatchChangeMods(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchChangeMods;
        public override Task Execute(UserToken token, byte[] data)
        {
            var mods = (Mods)ReadInt(data);
            var lobby = Multiplayer.GetFromUser(token.PlayerId);
            if(lobby is null) return Task.CompletedTask;
            if(lobby.FreeMode)
            {
                var slot = lobby.Slots.FirstOrDefault(x => x.Player.Id == token.PlayerId);
                slot!.Mods = mods;
            }
            else
            {
                lobby.Mods = mods;
            }
            Pool.ActionOn(lobby.Players.Select(x => x.Id), queue => queue.EnqueuePacket(new UpdateMatch(lobby)));
            return Task.CompletedTask;
        }
    }
}