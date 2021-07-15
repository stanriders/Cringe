using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.RequestPackets
{
    public class UserStatsRequest : RequestPacket
    {
        public UserStatsRequest(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.UserStatsRequest;

        public override async Task Execute(UserToken token, byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var length = (SkipToData(reader) - 2) / 4;
            var playerIds = ReadI32(reader, length);
            var players = await Task.WhenAll(playerIds.Select(x => Token.GetPlayerWithoutScores(x)));
            var pool = Pool;
            foreach (var player in players.Where(x => x is not null))
            {
                pool.ActionOn(token.PlayerId, queue => queue.EnqueuePacket(new UserStats(player.Stats)));
                pool.ActionOn(token.PlayerId, queue => queue.EnqueuePacket(new UserPresence(player.Presence)));
            }
        }
    }
}