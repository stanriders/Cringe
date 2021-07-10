using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Pages.Beatmaps
{
    public class DetailsModel : PageModel
    {
        private readonly BeatmapDatabaseContext _context;

        public DetailsModel(BeatmapDatabaseContext context)
        {
            _context = context;
        }

        public Beatmap Beatmap { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Beatmap = await _context.Beatmaps.FirstOrDefaultAsync(m => m.Id == id);

            if (Beatmap == null) return NotFound();
            return Page();
        }
    }
}