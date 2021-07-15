using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Services;
using Cringe.Types;
using Cringe.Types.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebController : ControllerBase
    {
        private readonly BeatmapDatabaseContext _beatmapContext;
        private readonly ScoreDatabaseContext _scoreDatabaseContext;
        private readonly ScoreService _scoreService;

        public WebController(ScoreService scoreService,
            ScoreDatabaseContext scoreDatabaseContext,
            BeatmapDatabaseContext beatmapContext)
        {
            _scoreService = scoreService;
            _scoreDatabaseContext = scoreDatabaseContext;
            _beatmapContext = beatmapContext;
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
        public async Task<IActionResult> SubmitScore([FromForm] string score,
            [FromForm] string iv,
            [FromForm] string osuver,
            [FromForm(Name = "x")] string quit,
            [FromForm(Name = "ft")] string failed)
        {
            var submittedScore = await _scoreService.SubmitScore(score, iv, osuver, quit == "1", failed == "1");
            if (submittedScore == null)
                return new OkResult();

            var outData =
                $"beatmapId:{submittedScore.BeatmapId}|beatmapSetId:2|beatmapPlaycount:3|beatmapPasscount:2|approvedDate:0\n" +
                new BeatmapChart
                {
                    Name = "AYE",
                    RankBefore = 2,
                    RankAfter = 1,
                    ScoreBefore = 100,
                    ScoreAfter = (ulong) submittedScore.Score,
                    ComboBefore = 1,
                    ComboAfter = (uint) submittedScore.MaxCombo,
                    AccuracyBefore = 0.2f,
                    AccuracyAfter = (float) submittedScore.Accuracy,
                    PpBefore = 0,
                    PpAfter = (ushort) submittedScore.Pp
                }
                +
                new PlayerChart
                {
                    Name = "Profile",
                    RankBefore = 2,
                    RankAfter = 1,
                    ScoreBefore = 0,
                    ScoreAfter = (ulong) submittedScore.Score,
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
        public async Task<IActionResult> GetScores([FromQuery(Name = "c")] string md5,
            [FromQuery(Name = "f")] string fileName,
            [FromQuery(Name = "i")] string beatmapSetId,
            [FromQuery(Name = "m")] GameModes? gameMode,
            [FromQuery(Name = "us")] string username,
            [FromQuery(Name = "ha")] string password,
            [FromQuery(Name = "v")] int? scoreboardType,
            [FromQuery(Name = "vv")] int? scoreboardVersion,
            [FromQuery(Name = "mods")] Mods? mods)
        {
            var beatmap = await _beatmapContext.Beatmaps.FirstOrDefaultAsync(x => x.Md5 == md5);
            if (beatmap is null)
                return new OkObjectResult($"{(int) RankedStatus.NotSubmitted}|false");

            var scores = await _scoreDatabaseContext.Scores
                .OrderByDescending(x => x.Score)
                .Where(x => x.BeatmapId == beatmap.Id && x.GameMode == gameMode)
                .ToArrayAsync();

            var data = $"{(int) RankedStatus.Ranked}|false|{beatmap.Id}|{beatmapSetId}|{scores.Length}\n0\naye\n10.0\n";

            for (var i = 0; i < scores.Length; i++)
                scores[i].LeaderboardPosition = i + 1;

            var userBest = scores.FirstOrDefault(x => x.PlayerUsername == username);
            if (userBest != null)
                data += userBest;
            else
                data += '\n';

            foreach (var score in scores)
                data += score;

            return new OkObjectResult(data);
        }
    }
}