using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Database;
using Cringe.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Services
{
    public class PlayersPool : IConnectable<UserToken, UserToken>
    {
        private readonly PlayerDatabaseContext _database;
        private readonly ILogger<PlayersPool> _logger;

        public PlayersPool(PlayerDatabaseContext database, ILogger<PlayersPool> logger)
        {
            _database = database;
            _logger = logger;
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

            _logger.LogDebug("{Token} | Connected to PlayersPool", token);
            _logger.LogDebug("Currently connected players:\n{Dump}", string.Join("|", GetPlayersId()));
            return true;
        }

        public bool Disconnect(UserToken token)
        {
            if (!Players.TryGetValue(token.PlayerId, out var playerSession)) return false;

            PlayerLoggedIn -= playerSession.PlayerLoggedIn;
            PlayerLoggedOut -= playerSession.PlayerLoggedOut;
            OnPlayerLoggedOut(playerSession);
            Players.Remove(token.PlayerId);
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
