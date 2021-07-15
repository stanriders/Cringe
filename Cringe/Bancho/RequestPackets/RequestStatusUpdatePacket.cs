using System;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class RequestStatusUpdatePacket : RequestPacket
    {
        public RequestStatusUpdatePacket(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.RequestStatusUpdate;

        public override async Task Execute(UserToken token, byte[] data)
        {
            var player = await Token.GetPlayerWithoutScores(token.PlayerId);
            Pool.ActionOn(token.PlayerId, queue => queue.EnqueuePacket(new UserStats(player.Stats)));
        }
    }
}