using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Services;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Pages.Beatmaps
{
    public class IndexModel : PageModel
    {
        private readonly BeatmapDatabaseContext _context;
        private readonly BeatmapService _beatmapService;

        public IndexModel(BeatmapDatabaseContext context, BeatmapService beatmapService)
        {
            _context = context;
            _beatmapService = beatmapService;
        }

        public IList<Beatmap> Beatmap { get; set; }

        public async Task OnGetAsync()
        {
            Beatmap = await _context.Beatmaps.OrderByDescending(x=>x.Id).Take(100).ToListAsync();
        }

        public IActionResult OnPost()
        {
            _beatmapService.SeedDatabse();
            return Page();
        }
    }
}