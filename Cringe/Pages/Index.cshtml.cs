using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PlayerDatabaseContext _playerContext;
        private readonly ScoreDatabaseContext _scoreContext;

        public IndexModel(PlayerDatabaseContext playerContext, ScoreDatabaseContext scoreContext)
        {
            _playerContext = playerContext;
            _scoreContext = scoreContext;
        }

        public IList<SubmittedScore> Scores { get; set; }

        public async Task OnGetAsync()
        {
            Scores = await _scoreContext.Scores.OrderByDescending(x => x.Pp).Take(100).ToListAsync();
        }
    }
}