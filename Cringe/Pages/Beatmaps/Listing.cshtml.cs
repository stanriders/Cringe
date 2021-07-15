using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Pages.Beatmaps
{
    public class IndexModel : PageModel
    {
        private readonly BeatmapDatabaseContext _context;

        public IndexModel(BeatmapDatabaseContext context)
        {
            _context = context;
        }

        public IList<Beatmap> Beatmap { get; set; }

        public async Task OnGetAsync()
        {
            Beatmap = await _context.Beatmaps.Take(1000).ToListAsync();
        }
    }
}