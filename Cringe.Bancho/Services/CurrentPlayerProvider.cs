using Cringe.Bancho.Types;
using Microsoft.AspNetCore.Http;

namespace Cringe.Bancho.Services;

public class CurrentPlayerProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentPlayerProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public PlayerSession Session => (PlayerSession) _httpContextAccessor.HttpContext!.Items["player-session"];
}
