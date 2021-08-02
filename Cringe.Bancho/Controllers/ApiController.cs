using System.Collections.Generic;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Microsoft.AspNetCore.Mvc;

namespace Cringe.Bancho.Controllers
{
    [Route("/api")]
    public class ApiController : ControllerBase
    {
        private readonly LobbyService _lobby;
        private readonly StatsService _stats;
        private readonly SpectateService _spectate;

        public ApiController(LobbyService lobby, StatsService stats, SpectateService spectate)
        {
            _lobby = lobby;
            _stats = stats;
            _spectate = spectate;
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
        public IActionResult UpdatePlayerStats(int playerId)
        {
            _stats.RemoveStats(playerId);

            PlayersPool.GetPlayer(playerId)?.UpdateStats();

            return Ok();
        }
    }
}
