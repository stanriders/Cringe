using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Pages.Scores
{
    public class EditModel : PageModel
    {
        private readonly ScoreDatabaseContext _context;

        public EditModel(ScoreDatabaseContext context)
        {
            _context = context;
        }

        [BindProperty] public SubmittedScore SubmittedScore { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            SubmittedScore = await _context.Scores.FirstOrDefaultAsync(m => m.Id == id);

            if (SubmittedScore == null) return NotFound();
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Attach(SubmittedScore).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubmittedScoreExists(SubmittedScore.Id))
                    return NotFound();
                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool SubmittedScoreExists(int id)
        {
            return _context.Scores.Any(e => e.Id == id);
        }
    }
}