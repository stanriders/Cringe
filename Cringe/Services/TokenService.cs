using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Cringe.Types.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Services
{
    public class TokenService
    {
        private readonly List<UserToken> tokens = new();

        public async Task<UserToken> AddToken(string username)
        {
            var existingToken = tokens.FirstOrDefault(x => x.Username == username);
            if (existingToken != null)
                return existingToken;

            await using var playerDb = new PlayerDatabaseContext();

            var player = await playerDb.Players.FirstOrDefaultAsync(x => x.Username == username);
            if (player == null)
            {
                player = new Player
                {
                    Username = username,
                    UserRank = UserRanks.Normal | UserRanks.Supporter,
                    Rank = 1
                };
                await playerDb.Players.AddAsync(player);
                await playerDb.SaveChangesAsync();
            }

            var token = new UserToken
            {
                PlayerId = player.Id,
                Token = Guid.NewGuid().ToString(),
                Username = username
            };
            tokens.Add(token);

            return token;
        }

        public UserToken GetToken(string token)
        {
            return tokens.FirstOrDefault(x => x.Token == token);
        }

        public async Task<Player> GetPlayer(string token)
        {
            var tokenData = tokens.FirstOrDefault(x => x.Token == token);
            if (tokenData == null)
                return null;

            await using var playerDb = new PlayerDatabaseContext();

            return await playerDb.Players.FirstOrDefaultAsync(x => x.Id == tokenData.PlayerId);
        }
    }
}