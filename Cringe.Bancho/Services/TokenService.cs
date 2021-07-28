using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Services;
using Cringe.Types;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Bancho.Services
{
    public class TokenService
    {
        private static readonly List<UserToken> _tokens = new();
        private readonly PlayerDatabaseContext _playerDatabaseContext;
        private readonly PlayerTopscoreStatsCache _ppCache;

        public TokenService(PlayerTopscoreStatsCache ppCache, PlayerDatabaseContext playerDatabaseContext)
        {
            _ppCache = ppCache;
            _playerDatabaseContext = playerDatabaseContext;
        }

        public async Task<UserToken> AddToken(string username)
        {
            var existingToken = _tokens.FirstOrDefault(x => x.Username == username);

            if (existingToken != null)
                return existingToken;

            var player = await _playerDatabaseContext.Players.FirstOrDefaultAsync(x => x.Username == username);

            if (player == null)
                return null;

            var token = new UserToken
            {
                PlayerId = player.Id,
                Token = Guid.NewGuid().ToString(),
                Username = username
            };
            _tokens.Add(token);

            return token;
        }

        public static UserToken GetToken(string token)
        {
            return _tokens.FirstOrDefault(x => x.Token == token);
        }
    }
}
