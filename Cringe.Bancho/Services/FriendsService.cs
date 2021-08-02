
using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Services
{
    public class FriendsService
    {
        private readonly PlayerDatabaseContext _database;
        private readonly ILogger<FriendsService> _logger;

        public FriendsService(PlayerDatabaseContext database, ILogger<FriendsService> logger)
        {
            _database = database;
            _logger = logger;
        }

        public async Task AddFriend(int from, int to)
        {
            if (!PlayersPool.Players.TryGetValue(from, out var fromSession) ||
                !PlayersPool.Players.TryGetValue(to, out var toSession))
            {
                _logger.LogWarning("AddFriend failed for {From}->{To} because some of them had no session!", from, to);
                return;
            }

            if (await _database.Friends.AnyAsync(x => x.From == fromSession.Player && x.To == toSession.Player))
            {
                _logger.LogWarning("AddFriend failed for {From}->{To} because they are already friends!", from, to);
                return;
            }

            await _database.Friends.AddAsync(new Friends
            {
                FromId = from,
                ToId = to
            });

            await _database.SaveChangesAsync();
        }

        public async Task RemoveFriend(int from, int to)
        {
            if (!PlayersPool.Players.TryGetValue(from, out var fromSession) ||
                !PlayersPool.Players.TryGetValue(to, out var toSession))
            {
                _logger.LogWarning("RemoveFriend failed for {From}->{To} because some of them had no session!", from, to);
                return;
            }

            var relation = await _database.Friends.Where(x => x.From == fromSession.Player && x.To == toSession.Player).SingleOrDefaultAsync();
            if (relation is null)
            {
                _logger.LogWarning("RemoveFriend failed for {From}->{To} because they were never friends!", from, to);
                return;
            }

            _database.Friends.Remove(relation);
            await _database.SaveChangesAsync();
        }
    }
}
