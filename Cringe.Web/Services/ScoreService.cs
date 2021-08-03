using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cringe.Database;
using Cringe.Services;
using Cringe.Types.Database;
using Cringe.Types.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Cringe.Web.Services
{
    public class ScoreService
    {
        private readonly BanchoApiWrapper _banchoApiWrapper;
        private readonly BeatmapDatabaseContext _beatmapContext;
        private readonly ILogger<ScoreService> _logger;
        private readonly PlayerDatabaseContext _playerContext;
        private readonly PpService _ppService;
        private readonly ScoreDatabaseContext _scoreContext;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public ScoreService(ScoreDatabaseContext scoreContext, PlayerDatabaseContext playerContext,
            BeatmapDatabaseContext beatmapContext, PpService ppService, BanchoApiWrapper banchoApiWrapper,
            ILogger<ScoreService> logger, IMapper mapper, IMemoryCache memoryCache)
        {
            _scoreContext = scoreContext;
            _playerContext = playerContext;
            _beatmapContext = beatmapContext;
            _ppService = ppService;
            _banchoApiWrapper = banchoApiWrapper;
            _logger = logger;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<ScoreBase> SubmitScore(string encodedData, string iv, string osuver, bool quit,
            bool failed)
        {
            var scoreData = DecryptScoreData(encodedData, iv, osuver);
            var scoreUniqueHash = $"{scoreData[0]}_{scoreData[1].Trim()}_{scoreData[9]}";

            if (_memoryCache.TryGetValue(scoreUniqueHash, out _))
            {
                _logger.LogInformation("Duplicate score {Score}", scoreUniqueHash);
                return null;
            }

            var score = await ProcessScoreData(scoreData);

            if (score is null)
            {
                _logger.LogWarning("Failed to decrypt a score!");

                return null;
            }

            _logger.LogDebug("Received score {Score}", score);

            // disallow duplicates for 1 minute
            _memoryCache.Set(scoreUniqueHash, 0, TimeSpan.FromMinutes(1));

            score.Quit = quit;
            score.Failed = !quit && failed;
            score.Pp = await _ppService.CalculatePp(score);

            await _scoreContext.RecentScores.AddAsync(_mapper.Map<RecentScore>(score));

            SubmittedScore submittedScore = null;

            if (score.Rank != "F" && !quit && !failed)
            {
                score.PreviousScore = await _scoreContext.Scores.AsNoTracking()
                    .Where(x => x.PlayerId == score.PlayerId && x.BeatmapId == score.BeatmapId)
                    .FirstOrDefaultAsync();

                // don't submit if previous score has bigger score
                if (score.PreviousScore?.Score > score.Score)
                    return null;

                if (score.PreviousScore is not null)
                    _scoreContext.Scores.Remove(score.PreviousScore);

                submittedScore = _mapper.Map<SubmittedScore>(score);
                await _scoreContext.Scores.AddAsync(submittedScore);

                // send score as a notif to confirm submission
                await _banchoApiWrapper.SendNotification(score.Player.Id, $"{Math.Round(score.Pp, 2)} pp");
                await _banchoApiWrapper.UpdatePlayerStats(score.Player.Id);
            }

            score.Player.Playcount++;
            score.Player.TotalScore += (ulong) score.Score;

            if (submittedScore is not null)
            {
                score.Id = submittedScore.Id;

                _logger.LogDebug($"Wrote score #{score.Id} to database");
            }

            await _playerContext.SaveChangesAsync();
            await _scoreContext.SaveChangesAsync();

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

        private async Task<ScoreBase> ProcessScoreData(string[] scoreData)
        {
            if (scoreData.Length >= 16 && scoreData[0].Length == 32)
            {
                var beatmap = await _beatmapContext.Beatmaps.FirstOrDefaultAsync(x => x.Md5 == scoreData[0]);

                if (beatmap is null)
                    return null;

                var player = await _playerContext.Players.FirstOrDefaultAsync(x => x.Username == scoreData[1].Trim());

                if (player is null)
                    return null;

                var submittedScore = new ScoreBase
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
                    PlayDateTime = DateTime.UtcNow,
                    OsuVersion = scoreData[17].Trim(),
                    PlayerId = player.Id,
                    PlayerUsername = player.Username,
                    BeatmapId = beatmap.Id,

                    // non-db models
                    Beatmap = beatmap,
                    Player = player
                };

                return submittedScore;
            }

            return null;
        }
    }
}
