using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.RequestPackets
{
    public class CreateMatch : RequestPacket
    {
        public CreateMatch(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.CreateMatch;
        public override async Task Execute(UserToken token, byte[] data)
        {
            var lobby = Lobby.Parse(data);
            lobby.Id = 5;
            var user = await Token.GetPlayerWithoutScores(token.PlayerId);
            lobby.Connect(user);
            Pool.ActionOn(token.PlayerId, queue =>
            {
                queue.EnqueuePacket(new MatchTransferHost());
                queue.EnqueuePacket(new UpdateMatch(lobby));
                queue.EnqueuePacket(new MatchJoinSuccess(lobby));
            });
        }
    }
}