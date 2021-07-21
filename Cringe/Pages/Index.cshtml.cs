using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Pages
{
    public class IndexModel : PageModel
    {
        private readonly BeatmapDatabaseContext _beatmapContext;
        private readonly ScoreDatabaseContext _scoreContext;

        public IndexModel(BeatmapDatabaseContext beatmapContext, ScoreDatabaseContext scoreContext)
        {
            _beatmapContext = beatmapContext;
            _scoreContext = scoreContext;
        }

        public IList<SubmittedScore> Scores { get; set; }

        public async Task OnGetAsync()
        {
            Scores = await _scoreContext.Scores.OrderByDescending(x => x.Pp).Take(50).ToListAsync();
            foreach (var score in Scores)
            {
                score.Beatmap = await _beatmapContext.Beatmaps.Where(x => x.Id == score.BeatmapId)
                    .Select(x => new Beatmap {Artist = x.Artist, Title = x.Title, DifficultyName = x.DifficultyName})
                    .FirstOrDefaultAsync();
            }
        }
    }
}