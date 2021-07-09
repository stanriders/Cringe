using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Pages.Players
{
    public class DetailsModel : PageModel
    {
        private readonly PlayerDatabaseContext _context;

        public DetailsModel(PlayerDatabaseContext context)
        {
            _context = context;
        }

        public Player Player { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Player = await _context.Players.FirstOrDefaultAsync(m => m.Id == id);

            if (Player == null) return NotFound();
            return Page();
        }
    }
}