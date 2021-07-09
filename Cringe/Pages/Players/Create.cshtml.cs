using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cringe.Pages.Players
{
    public class CreateModel : PageModel
    {
        private readonly PlayerDatabaseContext _context;

        public CreateModel(PlayerDatabaseContext context)
        {
            _context = context;
        }

        [BindProperty] public Player Player { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Players.Add(Player);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}