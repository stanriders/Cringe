using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly PlayerDatabaseContext _playerDatabaseContext;
        private readonly PlayerTopscoreStatsCache _ppCache;
        private readonly SpectateService _spectate;
        private readonly StatsService _stats;

        public ApiController(LobbyService lobby, StatsService stats, SpectateService spectate,
            PlayerTopscoreStatsCache ppCache, PlayerDatabaseContext playerDatabaseContext)
        {
            _lobby = lobby;
            _stats = stats;
            _spectate = spectate;
            _ppCache = ppCache;
            _playerDatabaseContext = playerDatabaseContext;
        }

        #region Spectators
        [HttpGet]
        [Route("spec/specs")]
        public IEnumerable<SpectateSession> GetSpecs()
        {
            return _spectate.Pool.Values;
        }
        #endregion

        #region Global
        [HttpGet]
        [Route("players/all/online")]
        public IEnumerable<int> GetOnlinePlayersId()
        {
            return PlayersPool.GetPlayersId();
        }

        [HttpPost]
        [Route("players/all/notification")]
        public IActionResult SendGlobalNotification(string text)
        {
            foreach (var session in PlayersPool.GetPlayerSessions())
                session.ReceiveNotification(text);

            return Ok();
        }
        #endregion

        #region Player
        [HttpGet]
        [Route("players/{playerId:int}")]
        public async Task<IActionResult> GetPlayer(int playerId)
        {
            var player = PlayersPool.GetPlayer(playerId).Player;

            if (player is not null) return Ok(player);

            player = await _playerDatabaseContext.Players.FirstOrDefaultAsync(x => x.Id == playerId);

            if (player is null) return NotFound();

            return Ok(player);
        }

        [HttpPost]
        [Route("players/{playerId:int}/notification")]
        public IActionResult SendIngameNotification(int playerId, string text)
        {
            var session = PlayersPool.GetPlayer(playerId);

            if (session is null) return BadRequest();

            session.ReceiveNotification(text);

            return Ok();
        }
        #endregion

        #region Stats
        [HttpGet]
        [Route("players/{playerId:int}/stats/refresh")]
        public async Task<IActionResult> UpdatePlayerStats(int playerId)
        {
            _stats.RemoveStats(playerId);

            var player = await _playerDatabaseContext.Players.FirstOrDefaultAsync(x => x.Id == playerId);
            await _ppCache.UpdatePlayerStats(player);
            await _playerDatabaseContext.SaveChangesAsync();

            PlayersPool.GetPlayer(playerId)?.UpdateStats(await _stats.GetUpdates(playerId));

            return Ok();
        }

        [HttpGet]
        [Route("players/{playerId:int}/stats")]
        public Task<Stats> GetStats(int playerId)
        {
            return _stats.GetUpdates(playerId);
        }
        #endregion

        #region Multiplayer
        [HttpGet]
        [Route("lobby/matches")]
        public IEnumerable<MatchSession> GetMatches()
        {
            return _lobby.Sessions.Values;
        }

        [HttpGet]
        [Route("lobby/matches/{playerId:int}")]
        public Task<IActionResult> GetMultiplayerMatch(int playerId)
        {
            return MatchWrapper(playerId, session => session);
        }

        [HttpGet]
        [Route("lobby/matches/{playerId:int}/is_host")]
        public Task<IActionResult> MultiplayerIsHost(int playerId)
        {
            return MatchWrapper(playerId, session => session.Match.Host == playerId);
        }

        [HttpGet]
        [Route("lobby/matches/{playerId:int}/status")]
        public Task<IActionResult> MultiplayerIsPlaying(int playerId)
        {
            return MatchWrapper(playerId, session => session.Match.GetPlayer(playerId).Status);
        }
        #endregion


        private async Task<IActionResult> MatchWrapper(int playerId, Func<MatchSession, object> selector)
        {
            var player = PlayersPool.GetPlayer(playerId);

            if (player?.MatchSession is not null) return Ok(selector(player.MatchSession));

            if (!await _playerDatabaseContext.Players.AnyAsync(x => x.Id == playerId))
                return NotFound();

            return NoContent();
        }
    }
}
