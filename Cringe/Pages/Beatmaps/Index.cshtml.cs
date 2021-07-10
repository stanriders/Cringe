using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cringe.Database;
using Cringe.Types;

namespace Cringe.Pages.Beatmaps
{
    public class IndexModel : PageModel
    {
        private readonly Cringe.Database.BeatmapDatabaseContext _context;

        public IndexModel(Cringe.Database.BeatmapDatabaseContext context)
        {
            _context = context;
        }

        public IList<Beatmap> Beatmap { get;set; }

        public async Task OnGetAsync()
        {
            Beatmap = await _context.Beatmaps.Take(1000).ToListAsync();
        }
    }
}
