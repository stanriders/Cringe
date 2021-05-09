using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using osuLocalBancho.Database;
using osuLocalBancho.Types;

namespace osuLocalBancho.Pages.Scores
{
    public class CreateModel : PageModel
    {
        private readonly osuLocalBancho.Database.ScoreDatabaseContext _context;

        public CreateModel(osuLocalBancho.Database.ScoreDatabaseContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public SubmittedScore SubmittedScore { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Scores.Add(SubmittedScore);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
