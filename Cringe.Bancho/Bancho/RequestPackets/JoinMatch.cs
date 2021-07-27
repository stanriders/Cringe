﻿using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets
{
    public class JoinMatch : RequestPacket
    {
        public JoinMatch(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ClientPacketType Type => ClientPacketType.JoinMatch;

        public override Task Execute(PlayerSession session, byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var id = reader.ReadInt32();
            if (session.MatchSession is not null)
            {
                session.Queue.EnqueuePacket(new MatchJoinFail());
                session.Queue.EnqueuePacket(new Notification("Sory bro server slomalsya :D"));
                Logger.LogCritical("{Token} | User connecting to the match while his MatchSession is not null ({MatchId})", session.Token, id);
                return Task.CompletedTask;
            }

            var match = Lobby.GetSession(id);
            match.Connect(session);

            Logger.LogInformation("{Token} | Connected to the match | {@Match}", session.Token, match);
            return Task.CompletedTask;
        }
    }
}
