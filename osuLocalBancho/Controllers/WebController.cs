using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using osuLocalBancho.Bancho;
using osuLocalBancho.Database;
using osuLocalBancho.Services;
using osuLocalBancho.Types;
using osuLocalBancho.Types.Enums;

namespace osuLocalBancho.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebController : ControllerBase
    {
        private readonly BanchoService _banchoService;

        public WebController(BanchoService banchoService)
        {
            _banchoService = banchoService;
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
        public IActionResult SubmitScore([FromForm] string score, [FromForm] string iv, [FromForm] string osuver, [FromForm] string x, [FromForm] string ft)
        {
            var key = "h89f2-890h2h89b34g-h80g134n90133";
            if (!string.IsNullOrEmpty(osuver))
            {
                key = $"osu!-scoreburgr---------{osuver}";
            }

            var decodedData = Convert.FromBase64String(score);
            var decodedIv = Convert.FromBase64String(iv);

            var scoreData = DecryptScoreData(decodedData, decodedIv, Encoding.Default.GetBytes(key));
            if (scoreData.Length >= 16 && scoreData[0].Length == 32)
            {
                var quit = x == "1";
                using var scoreDb = new ScoreDatabaseContext();
                scoreDb.Scores.Add(new SubmittedScore
                {
                    FileMd5 = scoreData[0],
                    PlayerUsername = scoreData[1],
                    // %s%s%s = scoreData[2]
                    Count300 = int.Parse(scoreData[3]),
                    Count100 = int.Parse(scoreData[4]),
                    Count50 = int.Parse(scoreData[5]),
                    CountGeki = int.Parse(scoreData[6]),
                    CountKatu = int.Parse(scoreData[7]),
                    CountMiss = int.Parse(scoreData[8]),
                    Score = int.Parse(scoreData[9]),
                    MaxCombo = int.Parse(scoreData[10]),
                    FullCombo = scoreData[11] == "True",
                    Rank = scoreData[12],
                    Mods = int.Parse(scoreData[13]),
                    Passed = scoreData[14] == "True",
                    GameMode = int.Parse(scoreData[15]),
                    //PlayDateTime = DateTime.Now,//int.Parse(scoreData[16]),
                    OsuVersion = scoreData[17],
                    Quit = quit,
                    Failed = !quit && int.Parse(ft) > 0,
                });
                scoreDb.SaveChanges();

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
                                  PpAfter = 727,
                              } +
                              new Chart
                              {
                                  Type = "overall",
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
                                  PpAfter = 727,
                              };

                // send score as a notif to confirm submission
                _banchoService.EnqueuePacket(DataPacket.PackData($"{scoreData[9]}"), ServerPacketType.Notification);

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

        private string[] DecryptScoreData(byte[] score, byte[] iv, byte[] key)
        {
            var engine = new RijndaelEngine(256);
            var blockCipher = new CbcBlockCipher(engine);
            var cipher = new PaddedBufferedBlockCipher(blockCipher, new ZeroBytePadding());
            var keyParam = new KeyParameter(key);
            var keyParamWithIv = new ParametersWithIV(keyParam, iv, 0, 32);

            cipher.Init(false, keyParamWithIv);
            var comparisonBytes = new byte[cipher.GetOutputSize(score.Length)];
            var length = cipher.ProcessBytes(score, comparisonBytes, 0);
            cipher.DoFinal(comparisonBytes, length);

            return Encoding.Default.GetString(comparisonBytes).Split(':');
        }
    }
}
