using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Web.Pages.Players
{
    public class IndexModel : PageModel
    {
        private readonly PlayerDatabaseContext _context;

        public IndexModel(PlayerDatabaseContext context)
        {
            _context = context;
        }

        public IList<Player> Player { get; set; }

        public async Task OnGetAsync()
        {
            Player = await _context.Players.AsNoTracking().OrderByDescending(x => x.Pp).ToListAsync();
        }
    }
}
