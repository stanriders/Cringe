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

        public ScoreService(ScoreDatabaseContext scoreContext, PlayerDatabaseContext playerContext)
        {
            _scoreContext = scoreContext;
            _playerContext = playerContext;
        }

        public async Task<SubmittedScore> SubmitScore(string score, string iv, string osuver, bool quit, bool failed)
        {
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
                if (player != null)
                {
                    if (!DateTime.TryParseExact(scoreData[16], "yyMMddhhmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                        date = DateTime.Now;

                    var submittedScore = new SubmittedScore
                    {
                        FileMd5 = scoreData[0],
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
                        PlayDateTime = date,
                        OsuVersion = scoreData[17].Trim(),
                        Quit = quit,
                        Failed = !quit && failed,
                        Player = player
                    };
                    await _scoreContext.Scores.AddAsync(submittedScore);
                    await _scoreContext.SaveChangesAsync();

                    if (scoreData[14] == "True")
                        player.Playcount++;

                    player.TotalScore += (ulong) submittedScore.Score;
                    await _playerContext.SaveChangesAsync();

                    return submittedScore;
                }
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