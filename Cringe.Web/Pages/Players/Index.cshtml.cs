﻿using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types.Database;
using Cringe.Types.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Web.Pages.Players
{
    public class DetailsModel : PageModel
    {
        private readonly BeatmapDatabaseContext _beatmapContext;
        private readonly PlayerDatabaseContext _context;
        private readonly ScoreDatabaseContext _scoreContext;

        public DetailsModel(PlayerDatabaseContext context, ScoreDatabaseContext scoreContext,
            BeatmapDatabaseContext beatmapContext)
        {
            _context = context;
            _scoreContext = scoreContext;
            _beatmapContext = beatmapContext;
        }

        public Player Player { get; set; }

        public SubmittedScore[] Scores { get; set; }

        public RecentScore[] RecentScores { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Player = await _context.Players.FirstOrDefaultAsync(m => m.Id == id);

            if (Player == null) return NotFound();

            Scores = await _scoreContext.Scores
                .Where(x => x.PlayerId == Player.Id && !x.Mods.HasFlag(Mods.Relax))
                .Take(100)
                .OrderByDescending(x => x.Pp)
                .ToArrayAsync();

            foreach (var score in Scores)
            {
                score.Beatmap = await _beatmapContext.Beatmaps.Where(x => x.Id == score.BeatmapId)
                    .Select(x => new Beatmap {Artist = x.Artist, Title = x.Title, DifficultyName = x.DifficultyName})
                    .FirstOrDefaultAsync();
            }

            RecentScores = await _scoreContext.RecentScores
                .Where(x => x.PlayerId == Player.Id)
                .Take(10)
                .OrderByDescending(x => x.PlayDateTime)
                .ToArrayAsync();

            foreach (var score in RecentScores)
            {
                score.Beatmap = await _beatmapContext.Beatmaps.Where(x => x.Id == score.BeatmapId)
                    .Select(x => new Beatmap { Artist = x.Artist, Title = x.Title, DifficultyName = x.DifficultyName })
                    .FirstOrDefaultAsync();
            }

            return Page();
        }
    }
}
