
using System.Threading.Tasks;
using Cringe.Services;
using Cringe.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cringe.Web.Controllers
{
    [Route("/")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private readonly BeatconnectApiWrapper _beatconnectApiWrapper;
        private readonly ILogger<RootController> _logger;

        public RootController(BeatconnectApiWrapper beatconnectApiWrapper, ILogger<RootController> logger)
        {
            _beatconnectApiWrapper = beatconnectApiWrapper;
            _logger = logger;
        }

        [Auth]
        [HttpGet("d/{id}")]
        public async Task<IActionResult> DownloadBeatmap(string id, [FromQuery(Name = "u")] string username, [FromQuery(Name = "h")] string password)
        {
            // ????????????
            if (id.EndsWith('n'))
                id = id[..^1];

            var intId = int.Parse(id);

            var setInfo = await _beatconnectApiWrapper.GetBeatmapSet(intId);
            if (setInfo is not null)
            {
                _logger.LogInformation("User {Username} is downloading beatmapset {Id}", username, id);

                await using var stream = await _beatconnectApiWrapper.DownloadBeatmapSet(intId, setInfo.UniqueId);
                return new FileStreamResult(stream, "application/x-osu-archive");
            }

            _logger.LogInformation("User {Username} failed to download beatmapset {Id}", username, id);

            return NotFound();
        }
    }
}
