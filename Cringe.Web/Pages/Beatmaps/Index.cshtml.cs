using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Web.Pages.Beatmaps
{
    public class DetailsModel : PageModel
    {
        private readonly BeatmapDatabaseContext _context;
        private readonly ScoreDatabaseContext _scoreContext;

        public DetailsModel(BeatmapDatabaseContext context, ScoreDatabaseContext scoreContext)
        {
            _context = context;
            _scoreContext = scoreContext;
        }

        public Beatmap Beatmap { get; set; }

        public SubmittedScore[] Scores { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Beatmap = await _context.Beatmaps.FirstOrDefaultAsync(m => m.Id == id);

            if (Beatmap == null) return NotFound();

            Scores = await _scoreContext.Scores
                .Where(x => x.BeatmapId == Beatmap.Id)
                .Take(100)
                .OrderByDescending(x => x.Score)
                .ToArrayAsync();

            return Page();
        }
    }
}