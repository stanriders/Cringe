﻿using System;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Match;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
{
    public class MatchFailed : RequestPacket
    {
        public MatchFailed(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.MatchFailed;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            if (session.MatchSession is null) return Task.CompletedTask;

            var packet = new MatchPlayerFailed(session.MatchSession.Match.GetPlayerPosition(session.Id));
            foreach (var player in session.MatchSession.Match.PlayingPlayers)
                player.Player.Queue.EnqueuePacket(packet);

            return Task.CompletedTask;
        }
    }
}
