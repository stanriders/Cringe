
using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Services;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cringe.Pages.Beatmaps
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
                    Md5 = apiMap.Md5
                });

                if (addedMap != null)
                {
                    await _context.SaveChangesAsync();
                    return RedirectToPage($"./{addedMap.Entity.Id}");
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
