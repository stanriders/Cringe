using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using osuLocalBancho.Types;

namespace osuLocalBancho.Pages.Players
{
    public class IndexModel : PageModel
    {
        private readonly osuLocalBancho.Database.PlayerDatabaseContext _context;

        public IndexModel(osuLocalBancho.Database.PlayerDatabaseContext context)
        {
            _context = context;
        }

        public IList<Player> Player { get;set; }

        public async Task OnGetAsync()
        {
            Player = await _context.Players.ToListAsync();
        }
    }
}
