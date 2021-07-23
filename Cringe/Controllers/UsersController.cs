using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Services;
using Cringe.Types;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc;

namespace Cringe.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly PlayerDatabaseContext _playerDatabaseContext;
        private readonly PlayersPool _pool;

        public UsersController(PlayersPool pool, PlayerDatabaseContext playerDatabaseContext)
        {
            _pool = pool;
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

            var queue = _pool.GetPlayer(player.Id).Queue;

            return queue.GetResult();
        }
    }
}