using System.Collections.Generic;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Database;
using Cringe.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Bancho.Controllers
{
    [Route("/api")]
    public class ApiController : ControllerBase
    {
        private readonly LobbyService _lobby;
        private readonly StatsService _stats;
        private readonly SpectateService _spectate;
        private readonly PlayerTopscoreStatsCache _ppCache;
        private readonly PlayerDatabaseContext _playerDatabaseContext;

        public ApiController(LobbyService lobby, StatsService stats, SpectateService spectate,
            PlayerTopscoreStatsCache ppCache, PlayerDatabaseContext playerDatabaseContext)
        {
            _lobby = lobby;
            _stats = stats;
            _spectate = spectate;
            _ppCache = ppCache;
            _playerDatabaseContext = playerDatabaseContext;
        }

        [HttpPost]
        [Route("notification")]
        public IActionResult SendIngameNotification(int playerId, string text)
        {
            var queue = PlayersPool.GetPlayer(playerId)?.Queue;

            if (queue is null) return BadRequest();

            queue.EnqueuePacket(new Notification(text));

            return Ok();
        }

        [HttpGet]
        [Route("players/ids")]
        public IEnumerable<int> GetPlayersId()
        {
            return PlayersPool.GetPlayersId();
        }

        [HttpGet]
        [Route("lobby/matches")]
        public IEnumerable<MatchSession> GetMatches()
        {
            return _lobby.Sessions.Values;
        }

        [HttpGet]
        [Route("spec/specs")]
        public IEnumerable<SpectateSession> GetSpecs()
        {
            return _spectate.Pool.Values;
        }

        [HttpPost]
        [Route("players/{playerId:int}/updateStats")]
        public async Task<IActionResult> UpdatePlayerStats(int playerId)
        {
            _stats.RemoveStats(playerId);

            var player = await _playerDatabaseContext.Players.FirstOrDefaultAsync(x=> x.Id == playerId);
            await _ppCache.UpdatePlayerStats(player);
            await _playerDatabaseContext.SaveChangesAsync();

            PlayersPool.GetPlayer(playerId)?.UpdateStats();

            return Ok();
        }

        [HttpGet]
        [Route("players/{playerId:int}/getStats")]
        public Task<Stats> GetStats(int playerId)
        {
            return _stats.GetUpdates(playerId);
        }
    }
}
