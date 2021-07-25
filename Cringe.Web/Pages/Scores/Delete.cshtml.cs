using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Web.Pages.Scores
{
    public class DeleteModel : PageModel
    {
        private readonly ScoreDatabaseContext _context;

        public DeleteModel(ScoreDatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SubmittedScore SubmittedScore { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            SubmittedScore = await _context.Scores.FirstOrDefaultAsync(m => m.Id == id);

            if (SubmittedScore == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            SubmittedScore = await _context.Scores.FindAsync(id);

            if (SubmittedScore != null)
            {
                _context.Scores.Remove(SubmittedScore);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
