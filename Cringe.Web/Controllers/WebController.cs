using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cringe.Attributes;
using Cringe.Database;
using Cringe.Services;
using Cringe.Types;
using Cringe.Types.Enums;
using Cringe.Web.Attributes;
using Cringe.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebController : ControllerBase
    {
        private readonly BeatmapDatabaseContext _beatmapContext;
        private readonly PlayerDatabaseContext _playerDatabaseContext;
        private readonly ReplayStorage _replayStorage;
        private readonly ScoreDatabaseContext _scoreDatabaseContext;
        private readonly ScoreService _scoreService;
        private readonly BeatconnectApiWrapper _beatconnectApiWrapper;
        private readonly OsuApiWrapper _osuApiWrapper;

        public WebController(ScoreService scoreService,
            ScoreDatabaseContext scoreDatabaseContext,
            BeatmapDatabaseContext beatmapContext,
            ReplayStorage replayStorage,
            PlayerDatabaseContext playerDatabaseContext,
            BeatconnectApiWrapper beatconnectApiWrapper,
            OsuApiWrapper osuApiWrapper)
        {
            _scoreService = scoreService;
            _scoreDatabaseContext = scoreDatabaseContext;
            _beatmapContext = beatmapContext;
            _replayStorage = replayStorage;
            _playerDatabaseContext = playerDatabaseContext;
            _beatconnectApiWrapper = beatconnectApiWrapper;
            _osuApiWrapper = osuApiWrapper;
        }

        [HttpPost("osu-error.php")]
        [HttpGet("lastfm.php")]
        [HttpGet("osu-getseasonal.php")]
        [HttpPost("osu-session.php")]
        [HttpGet("osu-markasread.php")]
        public IActionResult EmptyAnswer()
        {
            return new OkResult();
        }

        [Auth]
        [HttpGet("bancho_connect.php")]
        public IActionResult Connect([FromQuery(Name = "u")] string username, [FromQuery(Name = "h")] string password)
        {
            return new OkObjectResult("BR"); // ripple outputs player's country here, we output brazil
        }

        [Auth]
        [HttpGet("osu-getfriends.php")]
        public async Task<IActionResult> GetFriends([FromQuery(Name = "u")] string username, [FromQuery(Name = "h")] string password)
        {
            var friends = await _playerDatabaseContext.Friends
                .Where(x => x.From.Username == username)
                .Select(x => x.To.Id)
                .ToArrayAsync();

            return new FileContentResult(Encoding.UTF8.GetBytes(string.Join('\n', friends)),
                "text/html; charset=UTF-8");
        }

        [HttpPost("osu-submit-modular-selector.php")]
        public async Task<IActionResult> SubmitScore([FromForm] string score,
            [FromForm] string iv,
            [FromForm] string osuver,
            [FromForm(Name = "x")] string quit,
            [FromForm(Name = "ft")] int failTime,
            [FromForm(Name = "score")] IFormFile replay)
        {
            var submittedScore = await _scoreService.SubmitScore(score, iv, osuver, quit == "1", failTime > 0);

            if (submittedScore == null || submittedScore.Id == 0)
            {
                var outFailedData = "beatmapId:0|beatmapSetId:0|beatmapPlaycount:3|beatmapPasscount:2|approvedDate:0\n" +
                              new BeatmapChart() + new PlayerChart();

                return new OkObjectResult(outFailedData);
            }

            await using var replayStream = replay.OpenReadStream();
            await _replayStorage.SaveReplay(submittedScore.Id, replayStream);

            var outData =
                $"beatmapId:{submittedScore.BeatmapId}|beatmapSetId:2|beatmapPlaycount:3|beatmapPasscount:2|approvedDate:0\n" +
                new BeatmapChart
                {
                    Name = "AYE",
                    Url = $"https://bancho.stanr.info/beatmaps/{submittedScore.BeatmapId}",
                    RankBefore = 2,
                    RankAfter = 1,
                    ScoreBefore = (ulong) (submittedScore.PreviousScore?.Score ?? 0),
                    ScoreAfter = (ulong) submittedScore.Score,
                    ComboBefore = (uint) (submittedScore.PreviousScore?.MaxCombo ?? 0),
                    ComboAfter = (uint) submittedScore.MaxCombo,
                    AccuracyBefore = (float) (submittedScore.PreviousScore?.Accuracy ?? 0.0f),
                    AccuracyAfter = (float) submittedScore.Accuracy,
                    PpBefore = (ushort) (submittedScore.PreviousScore?.Pp ?? 0),
                    PpAfter = (ushort) submittedScore.Pp
                }
                +
                new PlayerChart
                {
                    Name = "Profile",
                    Url = $"https://bancho.stanr.info/players/{submittedScore.PlayerId}",
                    RankBefore = 99999,
                    RankAfter = submittedScore.Player.Rank,
                    ScoreBefore = submittedScore.Player.TotalScore - (ulong) submittedScore.Score,
                    ScoreAfter = submittedScore.Player.TotalScore,
                    AccuracyBefore = 0.0f,
                    AccuracyAfter = submittedScore.Player.Accuracy * 100.0f,
                    PpBefore = 0,
                    PpAfter = submittedScore.Player.Pp
                };

            return new OkObjectResult(outData);
        }

        //[Auth] // FIXME: its "us" and "ha" instead of "u" and "h"
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
            // TODO: check username/password

            var beatmap = await _beatmapContext.Beatmaps.FirstOrDefaultAsync(x => x.Md5 == md5);

            if (beatmap is null)
                return new OkObjectResult($"{(int) RankedStatus.NotSubmitted}|false");

            var scores = await _scoreDatabaseContext.Scores
                .OrderByDescending(x => x.Pp)
                .Where(x => x.BeatmapId == beatmap.Id && x.GameMode == gameMode)
                .ToArrayAsync();

            var data = new StringBuilder($"{(int) RankedStatus.Ranked}|false|{beatmap.Id}|{beatmapSetId}|{scores.Length}\n0\naye\n10.0\n");

            for (var i = 0; i < scores.Length; i++)
                scores[i].LeaderboardPosition = i + 1;

            var userBest = scores.FirstOrDefault(x => x.PlayerUsername == username);
            if (userBest != null)
                data.Append(userBest);
            else
                data.Append('\n');

            foreach (var score in scores)
                data.Append(score);

            return new OkObjectResult(data.ToString());
        }

        [Auth]
        [HttpGet("osu-getreplay.php")]
        public async Task<IActionResult> GetReplay([FromQuery(Name = "c")] int scoreId,
            [FromQuery(Name = "u")] string username,
            [FromQuery(Name = "h")] string password)
        {
            var replayData = await _replayStorage.GetRawReplay(scoreId);

            if (replayData is null)
                return NotFound();

            return new FileContentResult(replayData, "text/html; charset=UTF-8");
        }

        [Auth]
        [HttpGet("osu-search.php")]
        public async Task<IActionResult> BeatmapSetSearch([FromQuery(Name = "q")] string query,
            [FromQuery(Name = "m")] GameModes mode,
            [FromQuery(Name = "r")] int status,
            [FromQuery(Name = "u")] string username,
            [FromQuery(Name = "h")] string password)
        {
            var beatconnectMode = BeatconnectNamingAttribute.RetrieveNaming(mode);
            var beatconnectStatus = DirectStatusToBeatconnectStatus(status);

            if (query == "Newest")
                query = null;

            var beatmapsets = await _beatconnectApiWrapper.BeatmapSearch(query, beatconnectMode, beatconnectStatus);

            // {osz name}|{artist}|{title}|{mapper}|{status}|{rating}|{rank date}|{set id}|{thread id}|{has video}|{has storyboard}|{filesize}|{filesize_novid}|diffaname@0,diffname@0\n

            var responseString = $"{beatmapsets.Beatmaps.Length}\n";
            foreach (var beatmapSet in beatmapsets.Beatmaps)
            {
                responseString += $"{beatmapSet.Id}.osz|{beatmapSet.Artist}|{beatmapSet.Title}|{beatmapSet.Creator}|{(int)beatmapSet.Status}|10.00000|{beatmapSet.RankedDate}|{beatmapSet.Id}|0|0|{(beatmapSet.HasStoryboard ? 1 : 0)}|0||";
                foreach (var beatmap in beatmapSet.Beatmaps)
                {
                    responseString += $"{beatmap.DifficultyName}@0,";
                }

                responseString += '\n';
            }

            return new FileContentResult(Encoding.UTF8.GetBytes(responseString), "text/html; charset=UTF-8");
        }

        [Auth]
        [HttpGet("osu-search-set.php")]
        public async Task<IActionResult> BeatmapSearch([FromQuery(Name = "b")] int id,
            [FromQuery(Name = "u")] string username,
            [FromQuery(Name = "h")] string password)
        {
            var beatmap = await _osuApiWrapper.GetBeatmapInfo(id);
            if (beatmap is not null)
            {
                // {osz name}|{artist}|{title}|{mapper}|{status}|{rating?}|{update}|{set id}|{threadid}|{hasvideo}|{hasstoryboard}|{filesize}|{filesize_novid}

                var responseString =
                    $"{beatmap.BeatmapSetId}.osz|{beatmap.BeatmapSet.Artist}|{beatmap.BeatmapSet.Title}|{beatmap.BeatmapSet.CreatorName}|{beatmap.BeatmapSet.Status}|10.0|2021-05-12 18:33:23|{beatmap.BeatmapSetId}|0|0|0|0|";

                return new FileContentResult(Encoding.UTF8.GetBytes(responseString), "text/html; charset=UTF-8");
            }

            return NotFound();
        }

        private string DirectStatusToBeatconnectStatus(int status)
        {
            return status switch
            {
                2 => "pending",
                3 => "qualified",
                5 => "graveyard",
                8 => "loved",
                _ => "ranked"
            };
        }
    }
}
