
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Services
{
    public class PlayerRankCache
    {
        private readonly PlayerDatabaseContext _playerDatabaseContext;

        public PlayerRankCache(PlayerDatabaseContext playerDatabaseContext)
        {
            _playerDatabaseContext = playerDatabaseContext;
        }

        public async Task UpdatePlayerRank(Player player)
        {
            var rank = await RefreshRank(player.Id);
            if (rank != player.Rank)
            {
                player.Rank = rank;
            }
        }
        
        private async Task<uint> RefreshRank(int playerId)
        {
            var rank = await _playerDatabaseContext.PlayerRankQuery
                .FromSqlInterpolated($@"SELECT Rank FROM (SELECT ROW_NUMBER() OVER (ORDER BY Pp) Rank, Id FROM 'Players') WHERE Id = {playerId}")
                .FirstOrDefaultAsync();

            return rank.Rank;
        }
    }
}
