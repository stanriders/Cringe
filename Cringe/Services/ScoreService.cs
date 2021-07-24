using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cringe.Bancho.ResponsePackets;
using Cringe.Database;
using Cringe.Types.Database;
using Cringe.Types.Enums;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Cringe.Services
{
    public class ScoreService
    {
        private readonly BeatmapDatabaseContext _beatmapContext;
        private readonly PlayerDatabaseContext _playerContext;
        private readonly PlayersPool _pool;
        private readonly PlayerTopscoreStatsCache _ppCache;
        private readonly PpService _ppService;
        private readonly PlayerRankCache _rankCache;
        private readonly ScoreDatabaseContext _scoreContext;

        public ScoreService(ScoreDatabaseContext scoreContext, PlayerDatabaseContext playerContext,
            BeatmapDatabaseContext beatmapContext, PpService ppService, PlayersPool pool,
            PlayerTopscoreStatsCache ppCache, PlayerRankCache rankCache)
        {
            _pool = pool;
            _scoreContext = scoreContext;
            _playerContext = playerContext;
            _beatmapContext = beatmapContext;
            _ppService = ppService;
            _ppCache = ppCache;
            _rankCache = rankCache;
        }

        public async Task<SubmittedScore> SubmitScore(string encodedData, string iv, string osuver, bool quit,
            bool failed)
        {
            // TODO: recent scores
            if (quit || failed)
                return null;

            var key = "h89f2-890h2h89b34g-h80g134n90133";
            if (!string.IsNullOrEmpty(osuver))
                key = $"osu!-scoreburgr---------{osuver}";

            var decodedData = Convert.FromBase64String(encodedData);
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

                // this shouldn't happen since we check for quit & failed but it does
                if (scoreData[12] == "F")
                    return null;

                var score = long.Parse(scoreData[9]);

                var previousScore = await _scoreContext.Scores
                    .Where(x => x.PlayerId == player.Id && x.BeatmapId == beatmap.Id)
                    .FirstOrDefaultAsync();

                if (previousScore?.Score > score)
                    return null;

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
                    Score = score,
                    MaxCombo = int.Parse(scoreData[10]),
                    FullCombo = scoreData[11] == "True",
                    Rank = scoreData[12],
                    Mods = (Mods) Enum.Parse(typeof(Mods), scoreData[13]),
                    Passed = scoreData[14] == "True",
                    GameMode = (GameModes) Enum.Parse(typeof(GameModes), scoreData[15]),
                    PlayDateTime = date,
                    OsuVersion = scoreData[17].Trim(),
                    Quit = quit,
                    Failed = !quit && failed,
                    PlayerId = player.Id,
                    PlayerUsername = player.Username,
                    BeatmapId = beatmap.Id,
                    Beatmap = beatmap,
                    Player = player,
                    PreviousScore = previousScore
                };
                submittedScore.Pp = await _ppService.CalculatePp(submittedScore);

                if (previousScore is not null)
                    _scoreContext.Scores.Remove(previousScore);
                await _scoreContext.Scores.AddAsync(submittedScore);
                await _scoreContext.SaveChangesAsync();

                if (submittedScore.Passed)
                {
                    player.Playcount++;
                    await _ppCache.UpdatePlayerStats(player);
                    await _rankCache.UpdatePlayerRank(player);
                }

                player.TotalScore += (ulong) score;
                await _playerContext.SaveChangesAsync();

                // send score as a notif to confirm submission
                var queue = PlayersPool.GetPlayer(player.Id).Queue;
                queue.EnqueuePacket(new Notification($"{Math.Round(submittedScore.Pp, 2)} pp"));

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