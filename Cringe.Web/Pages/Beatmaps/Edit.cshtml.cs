using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Web.Pages.Beatmaps
{
    public class EditModel : PageModel
    {
        private readonly BeatmapDatabaseContext _context;

        public EditModel(BeatmapDatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RankedStatus Status { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var beatmap = await _context.Beatmaps.FirstOrDefaultAsync(m => m.Id == id);

            if (beatmap == null) return NotFound();

            Status = beatmap.Status;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid) return Page();

            if (id == null) return NotFound();

            var beatmap = await _context.Beatmaps.FirstOrDefaultAsync(m => m.Id == id);

            if (beatmap == null) return NotFound();

            beatmap.Status = Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BeatmapExists(id.Value))
                    return NotFound();

                throw;
            }

            return RedirectToPage("./Listing");
        }

        private bool BeatmapExists(int id)
        {
            return _context.Beatmaps.Any(e => e.Id == id);
        }
    }
}
