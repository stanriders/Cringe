using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Web.Pages.Scores
{
    public class IndexModel : PageModel
    {
        private readonly ScoreDatabaseContext _context;

        public IndexModel(ScoreDatabaseContext context)
        {
            _context = context;
        }

        public IList<SubmittedScore> SubmittedScore { get; set; }

        public async Task OnGetAsync()
        {
            SubmittedScore = await _context.Scores.AsNoTracking().OrderByDescending(x=> x.Id).Take(100).ToListAsync();
        }
    }
}
