using Cringe.Database;
using Cringe.Types.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Web.Pages.Scores
{
    public class FlaggedScore : SubmittedScore
    {
        public string Flags { get; set; }
    }

    public class FlagsModel : PageModel
    {
        private readonly ScoreDatabaseContext _context;
        private readonly FlagsService _flagsService;

        public FlagsModel(ScoreDatabaseContext context, FlagsService flagsService)
        {
            _context = context;
            _flagsService = flagsService;
        }

        public IList<FlaggedScore> SubmittedScore { get; set; }

        public async Task OnGetAsync()
        {
            var dbScores = await _context.Scores.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync();
            
            SubmittedScore = dbScores.Select(x => new FlaggedScore
            {
                Flags = _flagsService.DecodeFlags(x.OsuVersion).ToString(),
                Id = x.Id,
                PlayerId = x.PlayerId,
                PlayerUsername = x.PlayerUsername,
                BeatmapId = x.BeatmapId,
                Mods = x.Mods
            }).ToList();
        }
    }
}
