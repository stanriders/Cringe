using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Web.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly PlayerDatabaseContext _playerDatabaseContext;
        //private readonly PlayersPool _pool;

        public UsersController(PlayerDatabaseContext playerDatabaseContext)
        {
            //_pool = pool;
            _playerDatabaseContext = playerDatabaseContext;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterForm user, [FromForm] int check)
        {
            if (check == 0)
                return Ok();

            if (_playerDatabaseContext.Players.Any(x => x.Username == user.username))
                return BadRequest();

            var player = Player.Generate(user.username, user.password);

            await _playerDatabaseContext.Players.AddAsync(player);
            await _playerDatabaseContext.SaveChangesAsync();

            // queue = PlayersPool.GetPlayer(player.Id).Queue;

            //return queue.GetResult();

            return Ok();
        }
    }
}