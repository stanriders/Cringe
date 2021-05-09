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
    public class DetailsModel : PageModel
    {
        private readonly osuLocalBancho.Database.ScoreDatabaseContext _context;

        public DetailsModel(osuLocalBancho.Database.ScoreDatabaseContext context)
        {
            _context = context;
        }

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
    }
}
