using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Services;
using Cringe.Types;
using Microsoft.AspNetCore.Mvc;

namespace Cringe.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private BanchoServicePool _pool;

        public UsersController(BanchoServicePool pool)
        {
            _pool = pool;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterForm form)
        {
            if (form.check == 0)
                return Ok();
            await using var db = new PlayerDatabaseContext();
            if (db.Players.Any(x => x.Username == form.username)) return BadRequest();
            var user = await db.Players.AddAsync(Player.Generate(form.username, form.password));
            await db.SaveChangesAsync();
            var queue = _pool.GetFromPool(user.Entity.Id);
            //TODO: wtf?
            return queue.GetResult();
        }
    }
}