﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using osuLocalBancho.Types;

namespace osuLocalBancho.Pages.Players
{
    public class DetailsModel : PageModel
    {
        private readonly osuLocalBancho.Database.PlayerDatabaseContext _context;

        public DetailsModel(osuLocalBancho.Database.PlayerDatabaseContext context)
        {
            _context = context;
        }

        public Player Player { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Player = await _context.Players.FirstOrDefaultAsync(m => m.Id == id);

            if (Player == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
