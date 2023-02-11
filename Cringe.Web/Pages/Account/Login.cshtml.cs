using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;

namespace Cringe.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; private set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }
            
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        private readonly PlayerDatabaseContext _databaseContext;

        public LoginModel(PlayerDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            // Clear the existing external cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "АХХАХАХАХА КАКОЙ НАХУЙ ПАРОЛЬ");
                    return Page();
                }

                var user = await AuthenticateUser(Input.Username);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Кто");
                    return Page();
                }

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.Id.ToString()),
                    new("Username", user.Username),
                };

                claims.AddRange(user.UserRank.ToString().Split(',').Select(x=> new Claim(ClaimTypes.Role, x.Trim())));

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return LocalRedirect("/");
            }

            // Something failed. Redisplay the form.
            return Page();
        }

        private async Task<Player?> AuthenticateUser(string username)
        {
            return await _databaseContext.Players.FirstOrDefaultAsync(x => x.Username == username);
        }
    }
}
