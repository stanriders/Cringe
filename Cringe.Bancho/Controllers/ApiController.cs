using System.Collections.Generic;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Microsoft.AspNetCore.Mvc;

namespace Cringe.Bancho.Controllers
{
    public class ApiController : ControllerBase
    {
        private readonly LobbyService _lobby;

        public ApiController(LobbyService lobby)
        {
            _lobby = lobby;
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
    }
}
