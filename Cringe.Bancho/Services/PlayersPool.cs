﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Database;
using Cringe.Types;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Bancho.Services
{
    public class PlayersPool : IConnectable<UserToken, int>
    {
        private readonly PlayerDatabaseContext _database;

        public PlayersPool(PlayerDatabaseContext database)
        {
            _database = database;
        }

        public static Dictionary<int, PlayerSession> Players { get; } = new();

        public async Task<bool> Connect(UserToken token)
        {
            if (Players.ContainsKey(token.PlayerId)) return false;

            var player = await _database.Players.FirstOrDefaultAsync(x => x.Id == token.PlayerId);
            var session = new PlayerSession
            {
                Player = player,
                Token = token
            };

            OnPlayerLoggedIn(session);
            PlayerLoggedIn += session.PlayerLoggedIn;
            PlayerLoggedOut += session.PlayerLoggedOut;
            Players.Add(token.PlayerId, session);

            return true;
        }

        public bool Disconnect(int player)
        {
            if (!Players.TryGetValue(player, out var playerSession)) return false;

            PlayerLoggedIn -= playerSession.PlayerLoggedIn;
            PlayerLoggedOut -= playerSession.PlayerLoggedOut;
            OnPlayerLoggedOut(playerSession);
            Players.Remove(player);

            return true;
        }

        public event Action<PlayerSession> PlayerLoggedIn;
        public event Action<PlayerSession> PlayerLoggedOut;

        public IEnumerable<int> GetPlayersId()
        {
            return GetPlayerSessions().Select(x => x.Token.PlayerId);
        }

        public IEnumerable<PlayerSession> GetPlayerSessions()
        {
            return Players.Select(x => x.Value);
        }

        public static PlayerSession GetPlayer(int id)
        {
            Players.TryGetValue(id, out var session);

            return session;
        }

        public static PlayerSession GetPlayer(string username)
        {
            return Players.FirstOrDefault(x => x.Value.Player.Username == username).Value;
        }

        protected virtual void OnPlayerLoggedIn(PlayerSession obj)
        {
            PlayerLoggedIn?.Invoke(obj);
        }

        protected virtual void OnPlayerLoggedOut(PlayerSession obj)
        {
            PlayerLoggedOut?.Invoke(obj);
        }
    }
}
