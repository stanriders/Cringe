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
    public class IndexModel : PageModel
    {
        private readonly osuLocalBancho.Database.ScoreDatabaseContext _context;

        public IndexModel(osuLocalBancho.Database.ScoreDatabaseContext context)
        {
            _context = context;
        }

        public IList<SubmittedScore> SubmittedScore { get;set; }

        public async Task OnGetAsync()
        {
            SubmittedScore = await _context.Scores.ToListAsync();
        }
    }
}
