using System;
using System.IO;
using System.Threading.Tasks;
using Cringe.Types.Database;
using Microsoft.Extensions.Logging;
using OppaiSharp;
using Beatmap = OppaiSharp.Beatmap;

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
                using var reader = new StreamReader(stream, true);

                var map = Beatmap.Read(reader);
                var diff = new DiffCalc().Calc(map, (Mods) score.Mods);

                var pp = new PPv2(new PPv2Parameters(map, diff, score.Accuracy / 100, score.CountMiss, score.MaxCombo,
                    (Mods) score.Mods)).Total;

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
