using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Services
{
    public class TokenService
    {
        private static readonly List<UserToken> Tokens = new();
        private readonly PlayerDatabaseContext _playerDatabaseContext;
        private readonly PlayerTopscoreStatsCache _ppCache;

        public TokenService(PlayerTopscoreStatsCache ppCache, PlayerDatabaseContext playerDatabaseContext)
        {
            _ppCache = ppCache;
            _playerDatabaseContext = playerDatabaseContext;
        }

        public async Task<UserToken> AddToken(string username)
        {
            var existingToken = Tokens.FirstOrDefault(x => x.Username == username);
            if (existingToken != null)
                return existingToken;

            var player = await _playerDatabaseContext.Players.FirstOrDefaultAsync(x => x.Username == username);
            if (player == null) return null;

            var token = new UserToken
            {
                PlayerId = player.Id,
                Token = Guid.NewGuid().ToString(),
                Username = username
            };
            Tokens.Add(token);

            return token;
        }

        public UserToken GetToken(string token)
        {
            return Tokens.FirstOrDefault(x => x.Token == token);
        }

        public async Task<Player> GetPlayer(string token)
        {
            var tokenData = Tokens.FirstOrDefault(x => x.Token == token);
            if (tokenData == null)
                return null;

            var player = await _playerDatabaseContext.Players.FirstOrDefaultAsync(x => x.Id == tokenData.PlayerId);

            if (player != null)
            {
                var playerStats = await _ppCache.GetPlayerTopscoreStats(player.Id);
                if (player.Pp != playerStats.Pp)
                {
                    player.Pp = (ushort) playerStats.Pp;
                    player.Accuracy = (float) playerStats.Accuracy / 100.0f;
                    await _playerDatabaseContext.SaveChangesAsync();
                }
            }

            return player;
        }
    }
}