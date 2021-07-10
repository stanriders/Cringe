using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Cringe.Services
{
    public class ScoreService
    {
        private readonly PlayerDatabaseContext _playerContext;
        private readonly ScoreDatabaseContext _scoreContext;
        private readonly BeatmapDatabaseContext _beatmapContext;
        private readonly PpService _ppService;

        public ScoreService(ScoreDatabaseContext scoreContext, PlayerDatabaseContext playerContext, BeatmapDatabaseContext beatmapContext, PpService ppService)
        {
            _scoreContext = scoreContext;
            _playerContext = playerContext;
            _beatmapContext = beatmapContext;
            _ppService = ppService;
        }

        public async Task<SubmittedScore> SubmitScore(string score, string iv, string osuver, bool quit, bool failed)
        {
            // TODO: recent scores
            if (quit || failed)
                return null;

            var key = "h89f2-890h2h89b34g-h80g134n90133";
            if (!string.IsNullOrEmpty(osuver))
                key = $"osu!-scoreburgr---------{osuver}";

            var decodedData = Convert.FromBase64String(score);
            var decodedIv = Convert.FromBase64String(iv);

            var scoreData = DecryptScoreData(decodedData, decodedIv, Encoding.Default.GetBytes(key));
            if (scoreData.Length >= 16 && scoreData[0].Length == 32)
            {
                var username = scoreData[1].Trim();

                var player = await _playerContext.Players.FirstOrDefaultAsync(x => x.Username == username);
                if (player is null)
                    return null;

                var beatmap = await _beatmapContext.Beatmaps.FirstOrDefaultAsync(x => x.Md5 == scoreData[0]);
                if (beatmap is null)
                    return null;

                if (!DateTime.TryParseExact(scoreData[16], "yyMMddhhmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    date = DateTime.UtcNow;

                var submittedScore = new SubmittedScore
                {
                    Count300 = int.Parse(scoreData[3]),
                    Count100 = int.Parse(scoreData[4]),
                    Count50 = int.Parse(scoreData[5]),
                    CountGeki = int.Parse(scoreData[6]),
                    CountKatu = int.Parse(scoreData[7]),
                    CountMiss = int.Parse(scoreData[8]),
                    Score = long.Parse(scoreData[9]),
                    MaxCombo = int.Parse(scoreData[10]),
                    FullCombo = scoreData[11] == "True",
                    Rank = scoreData[12],
                    Mods = int.Parse(scoreData[13]),
                    Passed = scoreData[14] == "True",
                    GameMode = int.Parse(scoreData[15]),
                    PlayDateTime = date,
                    OsuVersion = scoreData[17].Trim(),
                    Quit = quit,
                    Failed = !quit && failed,
                    PlayerUsername = player.Username,
                    BeatmapId = beatmap.Id
                };
                submittedScore.Pp = await _ppService.CalculatePp(submittedScore);

                await _scoreContext.Scores.AddAsync(submittedScore);
                await _scoreContext.SaveChangesAsync();

                if (scoreData[14] == "True")
                    player.Playcount++;

                player.TotalScore += (ulong) submittedScore.Score;
                await _playerContext.SaveChangesAsync();

                return submittedScore;
            }

            return null;
        }

        public string[] DecryptScoreData(byte[] score, byte[] iv, byte[] key)
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