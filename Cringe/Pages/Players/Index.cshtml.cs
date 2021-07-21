using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Pages.Players
{
    public class DetailsModel : PageModel
    {
        private readonly PlayerDatabaseContext _context;
        private readonly ScoreDatabaseContext _scoreContext;

        public DetailsModel(PlayerDatabaseContext context, ScoreDatabaseContext scoreContext)
        {
            _context = context;
            _scoreContext = scoreContext;
        }

        public Player Player { get; set; }

        public SubmittedScore[] Scores { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Player = await _context.Players.FirstOrDefaultAsync(m => m.Id == id);

            if (Player == null) return NotFound();

            Scores = await _scoreContext.Scores
                .Where(x => x.PlayerId == Player.Id)
                .Take(100)
                .OrderByDescending(x => x.Pp)
                .ToArrayAsync();

            return Page();
        }
    }
}