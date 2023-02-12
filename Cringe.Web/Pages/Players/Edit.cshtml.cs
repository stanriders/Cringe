using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Cringe.Database;
using Cringe.Types.Database;
using Cringe.Types.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Web.Pages.Players
{
    public class PlayerEditModel
    {
        public string Username { get; set; }
        public UserRanks UserRank { get; set; }
    }

    public class EditModel : PageModel
    {
        private readonly PlayerDatabaseContext _context;
        private readonly IMapper _mapper;

        public EditModel(PlayerDatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public PlayerEditModel Player { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var dbPlayer = await _context.Players.FirstOrDefaultAsync(m => m.Id == id);
            if (dbPlayer == null) return NotFound();

            Player = _mapper.Map<PlayerEditModel>(dbPlayer);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid) return Page();

            if (id == null) return NotFound();

            var player = await _context.Players.FindAsync(id.Value);
            if (player == null) return NotFound();

            player.Username = Player.Username;
            player.UserRank = Player.UserRank;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExists(id.Value))
                    return NotFound();

                throw;
            }

            return RedirectToPage("./Listing");
        }

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.Id == id);
        }
    }
}
