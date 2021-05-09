using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using osuLocalBancho.Database;
using osuLocalBancho.Types;

namespace osuLocalBancho.Pages.Scores
{
    public class DeleteModel : PageModel
    {
        private readonly osuLocalBancho.Database.ScoreDatabaseContext _context;

        public DeleteModel(osuLocalBancho.Database.ScoreDatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SubmittedScore SubmittedScore { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SubmittedScore = await _context.Scores.FirstOrDefaultAsync(m => m.Id == id);

            if (SubmittedScore == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
