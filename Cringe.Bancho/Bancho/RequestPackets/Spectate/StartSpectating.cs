using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Spectate;

public class StartSpectatingRequest : RequestPacket, IRequest
{
    [PeppyField]
    public int Id { get; init; }

    public override ClientPacketType Type => ClientPacketType.StartSpectating;
}

public class StartSpectatingHandler : IRequestHandler<StartSpectatingRequest>
{
    private readonly ILogger<CantSpectateHandler> _logger;
    private readonly SpectateService _spectate;
    private readonly PlayerSession _session;

    public StartSpectatingHandler(ILogger<CantSpectateHandler> logger, CurrentPlayerProvider currentPlayerProvider,
        SpectateService spectate)
    {
        _logger = logger;
        _spectate = spectate;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(StartSpectatingRequest request, CancellationToken cancellationToken)
    {
        if (_session.SpectateSession is not null)
        {
            if (_session.SpectateSession.Host.Id == request.Id)
            {
                _logger.LogDebug("{Token} | Reconnecting to {@Spec}", _session.Token, _session.SpectateSession);
                _session.SpectateSession.Reconnect(_session);

                return Task.CompletedTask;
            }

            _logger.LogDebug("{Token} | Disconnecting from {@Spec}", _session.Token, _session.SpectateSession);
            _session.SpectateSession.Disconnect(_session);
        }

        var host = PlayersPool.GetPlayer(request.Id);
        if (host is null)
        {
            _logger.LogError("{Token} | Attempted to spectate offline of non-existing player {Id}", _session.Token,
                request.Id);

            return Task.CompletedTask;
        }

        _logger.LogDebug("{Token} | Connecting to {@Host}", _session.Token, host);
        _spectate.StartSpectating(host, _session);

        return Task.CompletedTask;
    }
}
