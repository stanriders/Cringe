﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class MatchSkipRequest : RequestPacket
    {
        private object key = new();
        public MatchSkipRequest(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchSkipRequest;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null) return Task.CompletedTask;

            var match = session.MatchSession.Match;
            var slot = match.GetPlayer(session.Token.PlayerId);
            slot.Skipped = true;
            var skipPacket = new MatchPlayerSkipped(match.GetPlayerPosition(session.Token.PlayerId));

            foreach (var player in match.PlayingPlayers)
            {
                player.Player.Queue.EnqueuePacket(skipPacket);
            }

            lock (key)
            {
                var players = match.PlayingPlayers.ToArray();
                if (players.Any(player => !player.Skipped))
                {
                    return Task.CompletedTask;
                }

                foreach (var player in match.PlayingPlayers)
                {
                    player.Player.Queue.EnqueuePacket(new MatchSkip());
                }
            }

            return Task.CompletedTask;
        }
    }
}