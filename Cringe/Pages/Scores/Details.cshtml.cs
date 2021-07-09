using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Pages.Scores
{
    public class DetailsModel : PageModel
    {
        private readonly ScoreDatabaseContext _context;

        public DetailsModel(ScoreDatabaseContext context)
        {
            _context = context;
        }

        public SubmittedScore SubmittedScore { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            SubmittedScore = await _context.Scores.FirstOrDefaultAsync(m => m.Id == id);

            if (SubmittedScore == null) return NotFound();
            return Page();
        }
    }
}