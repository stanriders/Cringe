using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types.Database;
using Cringe.Types.Enums;
using Cringe.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly BanchoApiWrapper _banchoApiWrapper;
        private readonly BeatmapDatabaseContext _beatmapContext;
        private readonly PlayerDatabaseContext _playerContext;
        private readonly ScoreDatabaseContext _scoreContext;

        public IndexModel(BeatmapDatabaseContext beatmapContext, ScoreDatabaseContext scoreContext,
            PlayerDatabaseContext playerContext, BanchoApiWrapper banchoApiWrapper)
        {
            _beatmapContext = beatmapContext;
            _scoreContext = scoreContext;
            _playerContext = playerContext;
            _banchoApiWrapper = banchoApiWrapper;
        }

        public IList<SubmittedScore> Scores { get; set; }

        public IList<string> OnlinePlayers { get; set; }

        public async Task OnGetAsync()
        {
            Scores = await _scoreContext.Scores.AsNoTracking()
                .OrderByDescending(x => x.Pp)
                .Where(x => !x.Mods.HasFlag(Mods.Relax) && !x.Mods.HasFlag(Mods.Relax2))
                .Take(50)
                .ToListAsync();

            foreach (var score in Scores)
                score.Beatmap = await _beatmapContext.Beatmaps.AsNoTracking()
                    .Where(x => x.Id == score.BeatmapId)
                    .Select(x => new Beatmap {Artist = x.Artist, Title = x.Title, DifficultyName = x.DifficultyName})
                    .FirstOrDefaultAsync();

            var onlinePlayers = await _banchoApiWrapper.GetOnlinePlayers();

            OnlinePlayers = await _playerContext.Players.AsNoTracking()
                .Where(x => onlinePlayers.Contains(x.Id))
                .Select(x => x.Username)
                .ToListAsync();
        }
    }
}
