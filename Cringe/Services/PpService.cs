using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Types.Database;
using Microsoft.Extensions.Logging;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.Beatmaps.Legacy;
using osu.Game.IO;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using osu.Game.Skinning;

namespace Cringe.Services
{
    public class PpService
    {
        private readonly BeatmapService _beatmapService;

        private readonly ILogger<PpService> _logger;

        //other game modes fuck you
        private static readonly OsuRuleset _ruleset = new();

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

                var beatmap = new StreamWorkingBeatmap(stream, score.BeatmapId);

                var calculator = new OsuPerformanceCalculator();

                var mods = _ruleset.ConvertFromLegacyMods((LegacyMods) (int) score.Mods).Append(new OsuModClassic());

                var difficultyAttributesCalculator = new OsuDifficultyCalculator(_ruleset.RulesetInfo, beatmap);
                var difficultyAttributes = (OsuDifficultyAttributes) difficultyAttributesCalculator.Calculate(mods);

                var scoreModel = new ScoreInfo(beatmap.BeatmapInfo, _ruleset.RulesetInfo)
                {
                    Statistics = new Dictionary<HitResult, int>
                    {
                        [HitResult.Great] = score.Count300,
                        [HitResult.Ok] = score.Count100,
                        [HitResult.Meh] = score.Count50,
                        [HitResult.Miss] = score.CountMiss,
                        [HitResult.SliderTailHit] = difficultyAttributes.SliderCount,
                        [HitResult.LargeTickMiss] = 0
                    },
                    MaxCombo = score.MaxCombo,
                    Accuracy = score.Accuracy,
                };

                var performanceAttributes = await calculator.CalculateAsync(scoreModel, difficultyAttributes, CancellationToken.None);

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

    class StreamWorkingBeatmap : WorkingBeatmap
    {
        private readonly IBeatmap _beatmap;
        public StreamWorkingBeatmap(Stream beatmapStream, int? beatmapId) : this(readFromStream(beatmapStream))
        {
            if (!beatmapId.HasValue)
                return;

            _beatmap.BeatmapInfo.OnlineID = beatmapId.Value;
        }

        public StreamWorkingBeatmap(IBeatmap beatmap) : base(beatmap.BeatmapInfo, null)
        {
            _beatmap = beatmap;
        }

        private static IBeatmap readFromStream(Stream beatmapStream)
        {
            using var lineBufferedReader = new LineBufferedReader(beatmapStream);

            return Decoder.GetDecoder<osu.Game.Beatmaps.Beatmap>(lineBufferedReader).Decode(lineBufferedReader);
        }
        protected override IBeatmap GetBeatmap() => _beatmap;
        public override Texture GetBackground()
        {
            throw new NotImplementedException();
        }
        public override Stream GetStream(string storagePath)
        {
            throw new NotImplementedException();
        }
        protected override Track GetBeatmapTrack()
        {
            throw new NotImplementedException();
        }
        protected override ISkin GetSkin()
        {
            throw new NotImplementedException();
        }
    }
}
