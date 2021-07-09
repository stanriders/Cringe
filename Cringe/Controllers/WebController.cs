using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Packets;
using Cringe.Database;
using Cringe.Services;
using Cringe.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebController : ControllerBase
    {
        private readonly BanchoServicePool _banchoServicePool;
        private readonly ScoreService _scoreService;

        public WebController(BanchoServicePool banchoServicePool, ScoreService scoreService)
        {
            _banchoServicePool = banchoServicePool;
            _scoreService = scoreService;
        }

        [HttpGet("bancho_connect.php")]
        [HttpPost("osu-error.php")]
        [HttpGet("osu-getfriends.php")]
        [HttpGet("lastfm.php")]
        [HttpGet("osu-getseasonal.php")]
        public IActionResult EmptyAnswer()
        {
            return new OkResult();
        }

        [HttpPost("osu-submit-modular-selector.php")]
        public async Task<IActionResult> SubmitScore([FromForm] string score, [FromForm] string iv,
            [FromForm] string osuver, [FromForm(Name = "x")] string quit, [FromForm(Name = "ft")] string failed)
        {
            var submittedScore = await _scoreService.SubmitScore(score, iv, osuver, quit == "1", failed == "1");
            if (submittedScore == null) return new OkResult();
            
            // send score as a notif to confirm submission
            var queue = _banchoServicePool.GetFromPool(submittedScore.Player.Id);
            queue.EnqueuePacket(new Notification(submittedScore.Id.ToString()));

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

        [HttpGet("osu-osz2-getscores.php")]
        public IActionResult GetScores([FromQuery(Name = "c")] string md5,
            [FromQuery(Name = "f")] string fileName,
            [FromQuery(Name = "i")] string beatmapSetId,
            [FromQuery(Name = "m")] int? gameMode,
            [FromQuery(Name = "us")] string username,
            [FromQuery(Name = "ha")] string password,
            [FromQuery(Name = "v")] int? scoreboardType,
            [FromQuery(Name = "vv")] int? scoreboardVersion,
            [FromQuery(Name = "mods")] int? mods)
        {
            var data = $"2|false|1488|{beatmapSetId}|1\n0\naye\n10.0\n";

            using var scoreDb = new ScoreDatabaseContext();
            var scores = scoreDb.Scores
                .OrderByDescending(x => x.Score)
                .Where(x => x.FileMd5 == md5 && x.GameMode == gameMode)
                .Include(x=> x.Player)
                .ToArray();

            for (var i = 0; i < scores.Length; i++)
                scores[i].LeaderboardPosition = i + 1;

            var userBest = scores.FirstOrDefault(x => x.Player.Username == username);
            if (userBest != null)
                data += userBest;
            else
                data += '\n';

            foreach (var score in scores)
                data += score;

            return new OkObjectResult(data.TrimEnd('\n'));
        }
    }
}
