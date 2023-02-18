using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class LogoutRequest : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.Logout;
}

public class Logout : IRequestHandler<LogoutRequest>
{
    private readonly PlayersPool _pool;
    private readonly ILogger<Logout> _logger;
    private readonly SpectateService _spectate;
    private readonly StatsService _stats;
    private readonly PlayerSession _session;

    public Logout(PlayersPool pool,
        CurrentPlayerProvider currentPlayerProvider,
        ILogger<Logout> logger,
        SpectateService spectate,
        StatsService stats)
    {
        _pool = pool;
        _session = currentPlayerProvider.Session;
        _logger = logger;
        _spectate = spectate;
        _stats = stats;
    }

    public Task Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        if (!_pool.Disconnect(_session.Token))
            _logger.LogWarning("{Token} | Failed to disconnect", _session.Token);

        _spectate.NukeOrLogout(_session);
        ChatService.Purge(_session);
        _stats.RemoveStats(_session.Id);

        _logger.LogInformation("{Token} | User logged out.\nConnected users are\n{Users}", _session.Token,
            string.Join(",", PlayersPool.GetPlayersId()));

        return Task.CompletedTask;
    }
}
