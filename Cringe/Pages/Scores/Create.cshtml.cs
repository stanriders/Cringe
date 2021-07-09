using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cringe.Pages.Scores
{
    public class CreateModel : PageModel
    {
        private readonly ScoreDatabaseContext _context;

        public CreateModel(ScoreDatabaseContext context)
        {
            _context = context;
        }

        [BindProperty] public SubmittedScore SubmittedScore { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Scores.Add(SubmittedScore);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}