using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Database;
using Cringe.Services;
using Cringe.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Services
{
    public class PlayersPool
    {
        private readonly PlayerDatabaseContext _database;
        private readonly ILogger<PlayersPool> _logger;
        private readonly PlayerTopscoreStatsCache _ppCache;

        public PlayersPool(PlayerDatabaseContext database, PlayerTopscoreStatsCache ppCache,
            ILogger<PlayersPool> logger)
        {
            _database = database;
            _ppCache = ppCache;
            _logger = logger;
        }

        public static ConcurrentDictionary<int, PlayerSession> Players { get; } = new();

        public async Task<bool> Connect(UserToken token)
        {
            if (Players.ContainsKey(token.PlayerId)) return false;

            var player = await _database.Players.FirstOrDefaultAsync(x => x.Id == token.PlayerId);

            await _ppCache.UpdatePlayerStats(player); //TODO: probably we shouldn't do this
            await _database.SaveChangesAsync();

            if (player is null) return false;

            var session = new PlayerSession
            {
                Player = player,
                Token = token
            };

            OnPlayerLoggedIn(session);
            PlayerLoggedIn += session.PlayerLoggedIn;
            PlayerLoggedOut += session.PlayerLoggedOut;
            if (!Players.TryAdd(token.PlayerId, session))
            {
                _logger.LogCritical("{Token} | Unable to add to a concurrent dictionary of player sessions", token);
            }

            _logger.LogDebug("{Token} | Connected to PlayersPool", token);
            _logger.LogDebug("Currently connected players:\n{Dump}", GetPlayersId());

            return true;
        }

        public bool Disconnect(UserToken token)
        {
            if (!Players.TryGetValue(token.PlayerId, out var playerSession)) return false;

            PlayerLoggedIn -= playerSession.PlayerLoggedIn;
            PlayerLoggedOut -= playerSession.PlayerLoggedOut;
            OnPlayerLoggedOut(playerSession);
            Players.Remove(token.PlayerId, out _);
            _logger.LogDebug("{Token} | Disconnected from PlayersPool", token);
            _logger.LogDebug("Currently connected players:\n{Dump}", string.Join("|", GetPlayersId()));

            return true;
        }

        public event Action<PlayerSession> PlayerLoggedIn;
        public event Action<PlayerSession> PlayerLoggedOut;

        public static IEnumerable<int> GetPlayersId()
        {
            return GetPlayerSessions().Select(x => x.Token.PlayerId);
        }

        public static IEnumerable<PlayerSession> GetPlayerSessions()
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
