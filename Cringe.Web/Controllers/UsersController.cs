using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Cringe.Types.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cringe.Web.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly PlayerDatabaseContext _playerDatabaseContext;

        public UsersController(PlayerDatabaseContext playerDatabaseContext, ILogger<UsersController> logger)
        {
            _playerDatabaseContext = playerDatabaseContext;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterForm user, [FromForm] int check)
        {
            _logger.LogDebug($"Trying to register player {user.username}");

            if (check == 0)
                return Ok();

            if (_playerDatabaseContext.Players.Any(x => x.Username == user.username))
            {
                _logger.LogWarning($"Player {user.username} tried registering, but username has been taken!");

                return BadRequest();
            }

            var player = Player.Generate(user.username, user.password);

            await _playerDatabaseContext.Players.AddAsync(player);
            await _playerDatabaseContext.SaveChangesAsync();

            _logger.LogInformation($"Registered player {player.Username} with ID {player.Id}");

            return Ok();
        }
    }
}
