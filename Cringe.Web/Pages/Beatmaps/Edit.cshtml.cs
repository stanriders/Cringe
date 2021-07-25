using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types.Database;
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
        public Beatmap Beatmap { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Beatmap = await _context.Beatmaps.FirstOrDefaultAsync(m => m.Id == id);

            if (Beatmap == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Attach(Beatmap).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BeatmapExists(Beatmap.Id))
                    return NotFound();

                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool BeatmapExists(int id)
        {
            return _context.Beatmaps.Any(e => e.Id == id);
        }
    }
}
