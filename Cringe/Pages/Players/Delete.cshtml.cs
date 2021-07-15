using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Pages.Players
{
    public class DeleteModel : PageModel
    {
        private readonly PlayerDatabaseContext _context;

        public DeleteModel(PlayerDatabaseContext context)
        {
            _context = context;
        }

        [BindProperty] public Player Player { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Player = await _context.Players.FirstOrDefaultAsync(m => m.Id == id);

            if (Player == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            Player = await _context.Players.FindAsync(id);

            if (Player != null)
            {
                _context.Players.Remove(Player);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}