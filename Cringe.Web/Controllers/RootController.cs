using System.IO;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Services;
using Cringe.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cringe.Web.Controllers
{
    [Route("/")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private readonly BeatconnectApiWrapper _beatconnectApiWrapper;
        private readonly BeatmapService _beatmapService;
        private readonly ILogger<RootController> _logger;

        public RootController(BeatconnectApiWrapper beatconnectApiWrapper, ILogger<RootController> logger, BeatmapService beatmapService)
        {
            _beatconnectApiWrapper = beatconnectApiWrapper;
            _logger = logger;
            _beatmapService = beatmapService;
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

                await using var inStream = await _beatconnectApiWrapper.DownloadBeatmapSet(intId, setInfo.UniqueId);
                await using var outStream = new MemoryStream();
                await inStream.CopyToAsync(outStream);
                outStream.Seek(0, SeekOrigin.Begin);

                return new FileStreamResult(outStream, "application/x-osu-archive");
            }

            _logger.LogInformation("User {Username} failed to download beatmapset {Id}", username, id);

            return NotFound();
        }

        [HttpPost("recalculate-scores")]
        public async Task<IActionResult> RecalculateScores([FromQuery(Name = "securityKey")] string securityKey,
            [FromServices] ScoreDatabaseContext databaseContext,
            [FromServices] PpService ppService,
            [FromServices] IConfiguration configuration)
        {
            if (securityKey != configuration["SecurityKey"]) return Unauthorized();

            _logger.LogWarning("Started PP recalculation");

            var submittedScores = await databaseContext.RecentScores.ToListAsync();

            foreach (var recentScore in submittedScores)
            {
                _logger.LogInformation("Recalculation: {ScoreId}", recentScore.Id);
                var newPp = await ppService.CalculatePp(recentScore);
                _logger.LogInformation("PP change {OldPp} -> {NewPp}", recentScore.Pp, newPp);
                recentScore.Pp = newPp;
            }

            await databaseContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedDatabase()
        {
            _logger.LogInformation("Starting database seeding...");

            await Task.Run(() => _beatmapService.SeedDatabse());

            return Ok();
        }
    }
}
