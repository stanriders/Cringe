using System.IO;
using System.Threading.Tasks;
using Cringe.Types;
using OppaiSharp;

namespace Cringe.Services
{
    public class PpService
    {
        private readonly BeatmapService _beatmapService;

        public PpService(BeatmapService beatmapService)
        {
            _beatmapService = beatmapService;
        }

        public async Task<double> CalculatePp(SubmittedScore score)
        {
            await using var stream = new MemoryStream(await _beatmapService.GetBeatmapBytes(score.BeatmapId), false);
            using var reader = new StreamReader(stream, true);

            var map = OppaiSharp.Beatmap.Read(reader);
            var diff = new DiffCalc().Calc(map, (Mods)score.Mods);

            return new PPv2(new PPv2Parameters(map, diff, score.Accuracy / 100, score.CountMiss, score.MaxCombo, (Mods)score.Mods)).Total;
        }
    }
}