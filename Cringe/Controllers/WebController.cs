using System.Threading.Tasks;
using Cringe.Bancho.Packets;
using Cringe.Services;
using Cringe.Types;
using Microsoft.AspNetCore.Mvc;

namespace Cringe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebController : ControllerBase
    {
        private readonly BanchoService _banchoService;
        private readonly ScoreService _scoreService;

        public WebController(BanchoService banchoService, ScoreService scoreService)
        {
            _banchoService = banchoService;
            _scoreService = scoreService;
        }

        [HttpGet("bancho_connect.php")]
        [HttpPost("osu-error.php")]
        [HttpGet("osu-getfriends.php")]
        [HttpGet("lastfm.php")]
        public IActionResult EmptyAnswer()
        {
            return new OkResult();
        }

        [HttpPost("osu-submit-modular-selector.php")]
        public async Task<IActionResult> SubmitScore([FromForm] string score, [FromForm] string iv,
            [FromForm] string osuver, [FromForm(Name = "x")] string quit, [FromForm(Name = "ft")] string failed)
        {
            var submittedScore = await _scoreService.SubmitScore(score, iv, osuver, quit == "1", failed == "1");
            if (submittedScore != null)
            {
                // send score as a notif to confirm submission
                _banchoService.EnqueuePacket(new Notification(submittedScore.Id.ToString()));

                var outData = "beatmapId:1|beatmapSetId:2|beatmapPlaycount:3|beatmapPasscount:2|approvedDate:0\n" +
                              new Chart
                              {
                                  Type = "beatmap",
                                  Name = "AYE",
                                  RankBefore = 2,
                                  RankAfter = 1,
                                  ScoreBefore = 100,
                                  ScoreAfter = 200,
                                  ComboBefore = 1,
                                  ComboAfter = 5,
                                  AccuracyBefore = 0.2f,
                                  AccuracyAfter = 0.33f,
                                  PpBefore = 666,
                                  PpAfter = 727
                              }
                              +
                              new Chart
                              {
                                  Type = "overall",
                                  Name = "AYE",
                                  RankBefore = 2,
                                  RankAfter = 1,
                                  //ScoreBefore = player.TotalScore,
                                  //ScoreAfter = player.TotalScore + (ulong)submittedScore.Score,
                                  ComboBefore = 1,
                                  ComboAfter = 5,
                                  AccuracyBefore = 0.2f,
                                  AccuracyAfter = 0.33f,
                                  PpBefore = 666,
                                  PpAfter = 727
                              };

                return new OkObjectResult(outData);
            }

            return new OkResult();
        }

        [HttpGet("osu-osz2-getscores.php")]
        public IActionResult GetScores([FromForm] string i)
        {
            var data = $"2|false|1488|{i}|1\n0\naye\n10\n";
            return new OkObjectResult(data);
        }
    }
}