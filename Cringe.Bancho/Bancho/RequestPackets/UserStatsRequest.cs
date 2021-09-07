using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class UserStatsRequest : RequestPacket
    {
        public UserStatsRequest(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.UserStatsRequest;
        protected override string ApiPath => "user/status/stats/request";

        public override async Task Execute(PlayerSession session, byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var playerIds = ReadI32(reader).ToArray();
            Logger.LogDebug("{Token} | Receive stats for these players: {Ids}", session.Token,
                string.Join(",", playerIds));
            var statsService = Stats;
            foreach (var playerId in playerIds)
            {
                if (playerId == session.Id) continue;

                var stats = await statsService.GetUpdates(playerId);

                if (stats is null) continue;

                session.Queue.EnqueuePacket(new UserStats(stats));
            }
        }
    }
}
