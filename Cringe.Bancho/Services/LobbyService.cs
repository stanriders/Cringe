﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Types;
using Cringe.Types;

namespace Cringe.Bancho.Services
{
    public class LobbyService : ISocial
    {
        public Dictionary<int, MatchSession> Sessions { get; set; } = new();

        public Task<bool> Connect(PlayerSession player)
        {
            NewMatch += player.NewMatch;
            DisposeMatch += player.DisposeMatch;
            UpdateMatch += player.UpdateMatch;
            foreach (var session in Sessions.Values)
                player.NewMatch(session.Match);

            return Task.FromResult(true);
        }

        public bool Disconnect(PlayerSession player)
        {
            NewMatch -= player.NewMatch;
            DisposeMatch -= player.DisposeMatch;
            UpdateMatch -= player.UpdateMatch;

            return true;
        }

        public MatchSession GetSession(int id)
        {
            Sessions.TryGetValue(id, out var res);

            return res;
        }

        public MatchSession CreateMatch(PlayerSession session, Match match)
        {
            var id = (short) (Sessions.Count + 1);
            var matchSession = new MatchSession(id, session, match, OnDisposeMatch);
            Sessions.Add(id, matchSession);
            matchSession.Connect(session);
            session.Queue.EnqueuePacket(new MatchTransferHost());
            session.MatchSession = matchSession;

            OnNewMatch(matchSession.Match);
            matchSession.UpdateMatch += OnUpdateMatch;

            return matchSession;
        }


        private event Action<Match> NewMatch;
        private event Action<Match> UpdateMatch;
        private event Action<Match> DisposeMatch;

        protected virtual void OnNewMatch(Match obj)
        {
            NewMatch?.Invoke(obj);
        }

        protected virtual void OnDisposeMatch(Match obj)
        {
            Sessions.Remove(obj.Id);
            DisposeMatch?.Invoke(obj);
        }

        protected virtual void OnUpdateMatch(Match obj)
        {
            UpdateMatch?.Invoke(obj);
        }
    }
}
