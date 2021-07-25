﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
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

        public override Task Execute(PlayerSession session, byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var playerIds = ReadI32(reader).ToArray();
            Logger.LogDebug("{Token} | Receive stats for these players: {Ids}", session.Token, string.Join(",", playerIds));
            foreach (var playerId in playerIds)
            {
                if (playerId == session.Token.PlayerId) continue;

                var player = PlayersPool.GetPlayer(playerId);

                if (player is null) continue;

                session.Queue.EnqueuePacket(new UserStats(player.GetStats()));
            }

            return Task.CompletedTask;
        }
    }
}
