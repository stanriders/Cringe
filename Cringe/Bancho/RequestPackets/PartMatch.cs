using System;
using System.Threading.Tasks;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class PartMatch : RequestPacket
    {
        public PartMatch(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.PartMatch;
        public override async Task Execute(UserToken token, byte[] data)
        {
            var player = await Token.GetPlayerWithoutScores(token.PlayerId);
            Multiplayer.NukePlayer(player);
        }
    }
}