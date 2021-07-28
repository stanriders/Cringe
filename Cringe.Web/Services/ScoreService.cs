using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Services;
using Cringe.Types.Database;
using Cringe.Types.Enums;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Cringe.Web.Services
{
    public class ScoreService
    {
        private readonly BeatmapDatabaseContext _beatmapContext;
        private readonly PlayerDatabaseContext _playerContext;
        private readonly PlayerTopscoreStatsCache _ppCache;
        private readonly PpService _ppService;
        private readonly PlayerRankCache _rankCache;
        private readonly ScoreDatabaseContext _scoreContext;
        private readonly BanchoApiWrapper _banchoApiWrapper;

        public ScoreService(ScoreDatabaseContext scoreContext, PlayerDatabaseContext playerContext,
            BeatmapDatabaseContext beatmapContext, PpService ppService, PlayerTopscoreStatsCache ppCache,
            PlayerRankCache rankCache, BanchoApiWrapper banchoApiWrapper)
        {
            _scoreContext = scoreContext;
            _playerContext = playerContext;
            _beatmapContext = beatmapContext;
            _ppService = ppService;
            _ppCache = ppCache;
            _rankCache = rankCache;
            _banchoApiWrapper = banchoApiWrapper;
        }

        public async Task<SubmittedScore> SubmitScore(string encodedData, string iv, string osuver, bool quit, bool failed)
        {
            // TODO: recent scores
            if (quit || failed)
                return null;

            var score = await ProcessScoreData(DecryptScoreData(encodedData, iv, osuver));

            if (score is null)
                return null;

            // don't submit if previous score has bigger score
            if (score.PreviousScore?.Score > score.Score)
                return null;

            // this shouldn't happen since we check for quit & failed but it does
            if (score.Rank == "F")
                return null;

            score.Quit = quit;
            score.Failed = !quit && failed;
            score.Pp = await _ppService.CalculatePp(score);

            if (score.PreviousScore is not null)
                _scoreContext.Scores.Remove(score.PreviousScore);

            await _scoreContext.Scores.AddAsync(score);
            await _scoreContext.SaveChangesAsync();

            if (score.Passed)
            {
                score.Player.Playcount++;
                await _ppCache.UpdatePlayerStats(score.Player);
                await _rankCache.UpdatePlayerRank(score.Player);
            }

            score.Player.TotalScore += (ulong) score.Score;
            await _playerContext.SaveChangesAsync();

            // send score as a notif to confirm submission
            await _banchoApiWrapper.SendNotification(score.Player.Id, $"{Math.Round(score.Pp, 2)} pp");

            return score;
        }

        public string[] DecryptScoreData(string score, string iv, string osuver)
        {
            var key = "h89f2-890h2h89b34g-h80g134n90133";
            if (!string.IsNullOrEmpty(osuver))
                key = $"osu!-scoreburgr---------{osuver}";

            var keyBytes = Encoding.Default.GetBytes(key);

            var decodedData = Convert.FromBase64String(score);
            var decodedIv = Convert.FromBase64String(iv);

            var engine = new RijndaelEngine(256);
            var blockCipher = new CbcBlockCipher(engine);
            var cipher = new PaddedBufferedBlockCipher(blockCipher, new ZeroBytePadding());
            var keyParam = new KeyParameter(keyBytes);
            var keyParamWithIv = new ParametersWithIV(keyParam, decodedIv, 0, 32);

            cipher.Init(false, keyParamWithIv);
            var comparisonBytes = new byte[cipher.GetOutputSize(decodedData.Length)];
            var length = cipher.ProcessBytes(decodedData, comparisonBytes, 0);
            cipher.DoFinal(comparisonBytes, length);

            return Encoding.Default.GetString(comparisonBytes).Split(':');
        }

        private async Task<SubmittedScore> ProcessScoreData(string[] scoreData)
        {
            if (scoreData.Length >= 16 && scoreData[0].Length == 32)
            {
                var beatmap = await _beatmapContext.Beatmaps.FirstOrDefaultAsync(x => x.Md5 == scoreData[0]);
                if (beatmap is null)
                    return null;

                var player = await _playerContext.Players.FirstOrDefaultAsync(x => x.Username == scoreData[1].Trim());
                if (player is null)
                    return null;

                var previousScore = await _scoreContext.Scores
                    .Where(x => x.PlayerId == player.Id && x.BeatmapId == beatmap.Id)
                    .FirstOrDefaultAsync();

                if (!DateTime.TryParseExact(scoreData[16], "yyMMddHHmmss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date))
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
                    Mods = (Mods) Enum.Parse(typeof(Mods), scoreData[13]),
                    Passed = scoreData[14] == "True",
                    GameMode = (GameModes) Enum.Parse(typeof(GameModes), scoreData[15]),
                    PlayDateTime = date,
                    OsuVersion = scoreData[17].Trim(),
                    PlayerId = player.Id,
                    PlayerUsername = player.Username,
                    BeatmapId = beatmap.Id,

                    // non-db models
                    Beatmap = beatmap,
                    Player = player,
                    PreviousScore = previousScore
                };

                return submittedScore;
            }

            return null;
        }
    }
}
