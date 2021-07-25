using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Services;
using Cringe.Types.Database;
using Cringe.Types.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cringe.Web.Pages.Beatmaps
{
    public class AddModel : PageModel
    {
        private readonly BeatmapDatabaseContext _context;
        private readonly OsuApiWrapper _osuApiWrapper;

        public AddModel(BeatmapDatabaseContext context, OsuApiWrapper osuApiWrapper)
        {
            _context = context;
            _osuApiWrapper = osuApiWrapper;
        }

        [BindProperty]
        public int? BeatmapId { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (BeatmapId == null || BeatmapId == 0) return Page();

            if (_context.Beatmaps.Any(e => e.Id == BeatmapId)) return BadRequest();

            var apiMap = await _osuApiWrapper.GetBeatmapInfo(BeatmapId.Value);
            if (apiMap != null)
            {
                var addedMap = await _context.Beatmaps.AddAsync(new Beatmap
                {
                    Id = BeatmapId.Value,
                    BeatmapSetId = apiMap.BeatmapSetId,
                    Mode = apiMap.Mode,
                    Md5 = apiMap.Md5,
                    Status = RankedStatus.Ranked,
                    Artist = apiMap.BeatmapSet?.Artist,
                    Title = apiMap.BeatmapSet?.Title,
                    DifficultyName = apiMap.Version,
                    Creator = apiMap.BeatmapSet?.CreatorName,
                    Bpm = apiMap.BPM,
                    HpDrain = apiMap.HP,
                    CircleSize = apiMap.CS,
                    OverallDifficulty = apiMap.OD,
                    ApproachRate = apiMap.AR,
                    Length = (int) apiMap.DrainLength
                });

                if (addedMap != null)
                {
                    await _osuApiWrapper.DownloadBeatmap(BeatmapId.Value);
                    await _context.SaveChangesAsync();

                    return Redirect(Url.Page("Index", new {id = BeatmapId.Value}));
                }
            }

            return Page();
        }
    }
}
