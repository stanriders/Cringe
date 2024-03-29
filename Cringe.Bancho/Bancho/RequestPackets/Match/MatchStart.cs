﻿using System;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
{
    public class MatchStart : RequestPacket
    {
        public MatchStart(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchStart;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null)
            {
                Logger.LogError("{Token} | MatchStart packet while not in match", session.Token);

                return Task.CompletedTask;
            }

            var match = session.MatchSession.Match;
            if (match.Host != session.Id)
            {
                Logger.LogInformation("{Token} | Attempted to start match as non-host", session.Token);

                return Task.CompletedTask;
            }

            match.InProgress = true;
            foreach (var player in match.OccupiedSlots)
            {
                if (player.Status == SlotStatus.NoMap) continue;

                player.Status = SlotStatus.Playing;
            }

            var response = new ResponsePackets.Match.MatchStart(match);
            foreach (var player in match.OccupiedSlots)
            {
                if (player.Status != SlotStatus.Playing) continue;

                player.Player.Queue.EnqueuePacket(response);
            }

            session.MatchSession.OnUpdateMatch(true);

            return Task.CompletedTask;
        }
    }
}
