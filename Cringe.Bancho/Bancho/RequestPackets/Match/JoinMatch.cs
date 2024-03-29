﻿using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Bancho.ResponsePackets.Match;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Match
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
            var password = ReadString(reader.BaseStream);

            if (session.MatchSession is not null)
            {
                session.Queue.EnqueuePacket(new MatchJoinFail());
                session.Queue.EnqueuePacket(new Notification("Sory bro server slomalsya :D"));
                Logger.LogCritical(
                    "{Token} | User connecting to the match while his MatchSession is not null ({MatchId})",
                    session.Token, id);

                return Task.CompletedTask;
            }

            var match = Lobby.GetSession(id);
            if (match is null)
            {
                session.Queue.EnqueuePacket(new MatchJoinFail());
                session.Queue.EnqueuePacket(new Notification("Lobbeshnika nema, on nyuknulsya D:"));

                return Task.CompletedTask;
            }

            if (match.ConnectWithPassword(session, password))
                Logger.LogInformation("{Token} | Connected to the match | {@Match}", session.Token, match);
            else
                Logger.LogInformation(
                    "{Token} | Tried to connect to match with wrong password ({HisPassword}) ({RealPassword})",
                    session.Token, password, match.Match.Password);

            return Task.CompletedTask;
        }
    }
}
