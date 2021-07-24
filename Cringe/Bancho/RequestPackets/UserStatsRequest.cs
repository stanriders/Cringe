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

        public override Task Execute(PlayerSession session, byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var playerIds = ReadI32(reader);
            var pool = Pool;
            foreach (var playerId in playerIds)
            {
                if(playerId == session.Token.PlayerId) continue;
                
                var player = pool.GetPlayer(playerId);
                if (player is null) continue;

                session.Queue.EnqueuePacket(new UserStats(player.Player.Stats));
            }

            return Task.CompletedTask;
        }
    }
}