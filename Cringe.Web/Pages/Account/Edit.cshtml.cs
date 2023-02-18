using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cringe.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Cringe.Web.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly PlayerDatabaseContext _context;
        private readonly IConfiguration _configuration;

        public EditModel(IConfiguration configuration, PlayerDatabaseContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public IFormFile Avatar { get; set; }

        public async Task<IActionResult> OnPostAvatar()
        {
            if (!ModelState.IsValid) return Page();

            if (!Avatar.ContentType.StartsWith("image"))
            {
                ModelState.AddModelError("Avatar", "Это не картинка");
                return Page();
            }

            if (Avatar.Length > 262144)
            {
                ModelState.AddModelError("Avatar", "Жирновато");
                return Page();
            }

            try
            {
                var path = Path.Combine(_configuration["AvatarStoragePath"], $"{User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value}.png");

                await using var file = System.IO.File.OpenWrite(path);
                await Avatar.CopyToAsync(file);
            }
            catch (Exception)
            {
                // dont care
            }

            return LocalRedirect("/");
        }

        public async Task<IActionResult> OnPostUsername()
        {
            if (!ModelState.IsValid) return Page();
            
            var player = await _context.Players.FindAsync(Convert.ToInt32(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value));
            if (player == null) return NotFound();

            player.Username = Username;

            await _context.SaveChangesAsync();

            await HttpContext.SignOutAsync();

            return LocalRedirect("/");
        }
    }
}
