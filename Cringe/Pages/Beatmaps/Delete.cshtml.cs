using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Pages.Beatmaps
{
    public class DeleteModel : PageModel
    {
        private readonly BeatmapDatabaseContext _context;

        public DeleteModel(BeatmapDatabaseContext context)
        {
            _context = context;
        }

        [BindProperty] public Beatmap Beatmap { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Beatmap = await _context.Beatmaps.FirstOrDefaultAsync(m => m.Id == id);

            if (Beatmap == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            Beatmap = await _context.Beatmaps.FindAsync(id);

            if (Beatmap != null)
            {
                _context.Beatmaps.Remove(Beatmap);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}