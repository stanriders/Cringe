using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Types.Database;
using Microsoft.Extensions.Logging;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.Beatmaps.Legacy;
using osu.Game.IO;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;

namespace Cringe.Services
{
    public class PpService
    {
        private readonly BeatmapService _beatmapService;

        private readonly ILogger<PpService> _logger;


        public PpService(BeatmapService beatmapService, ILogger<PpService> logger)
        {
            _beatmapService = beatmapService;
            _logger = logger;
        }

        public async Task<double> CalculatePp(ScoreBase score)
        {
            try
            {
                await using var stream =
                    new MemoryStream(await _beatmapService.GetBeatmapBytes(score.BeatmapId), false);

                using var lineBufferedReader = new LineBufferedReader(stream);
                var beatmap = Decoder.GetDecoder<osu.Game.Beatmaps.Beatmap>(lineBufferedReader).Decode(lineBufferedReader);
                var workingBeatmap = new FlatWorkingBeatmap(beatmap);

                OsuRuleset osuRuleset = new();
                var mods = osuRuleset.ConvertFromLegacyMods((LegacyMods) (int) score.Mods).Append(new OsuModClassic());

                var scoreModel = new ScoreInfo(beatmap.BeatmapInfo, osuRuleset.RulesetInfo)
                {
                    Statistics = new Dictionary<HitResult, int>
                    {
                        [HitResult.Great] = score.Count300,
                        [HitResult.Ok] = score.Count100,
                        [HitResult.Meh] = score.Count50,
                        [HitResult.Miss] = score.CountMiss
                    },
                    MaxCombo = score.MaxCombo,
                    Accuracy = score.Accuracy,
                    Mods = mods.ToArray()
                };

                var performanceAttributes = osuRuleset.CreatePerformanceCalculator().Calculate(scoreModel, workingBeatmap);

                var pp = performanceAttributes.Total;

                if (!double.IsNormal(pp))
                    pp = 0.0;

                return pp;
            }
            catch (Exception e)
            {
                _logger.LogError($"PP calculation failed: {e}");

                return 0.00;
            }
        }
    }
}
