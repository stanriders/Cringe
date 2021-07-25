using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cringe.Bancho.Controllers
{
    public class ApiController : ControllerBase
    {
        [HttpPost]
        [Route("notification")]
        public IActionResult SendIngameNotification(int playerId, string text)
        {
            var queue = PlayersPool.GetPlayer(playerId).Queue;
            if (queue is not null)
            {
                queue.EnqueuePacket(new Notification(text));

                return Ok();
            }

            return BadRequest();
        }
    }
}
