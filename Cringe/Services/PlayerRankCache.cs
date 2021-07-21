using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types.Database;
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
            if (rank != player.Rank) player.Rank = rank;
        }

        private async Task<uint> RefreshRank(int playerId)
        {
            var rank = await _playerDatabaseContext.PlayerRankQuery
                .FromSqlInterpolated($@"SELECT ROW_NUMBER() OVER (ORDER BY Pp DESC) Rank, Id FROM 'Players'")
                .Where(x => x.Id == playerId)
                .FirstOrDefaultAsync();

            return rank.Rank;
        }
    }
}