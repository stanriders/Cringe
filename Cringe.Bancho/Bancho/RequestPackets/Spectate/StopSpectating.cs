using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Spectate;

public class StopSpectatingCommand : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.StopSpectating;
}

public class StopSpectatingHandler : IRequestHandler<StopSpectatingCommand>
{
    private readonly ILogger<CantSpectateHandler> _logger;
    private readonly PlayerSession _session;

    public StopSpectatingHandler(ILogger<CantSpectateHandler> logger, CurrentPlayerProvider currentPlayerProvider)
    {
        _logger = logger;
        _session = currentPlayerProvider.Session;
    }

    public Task<Unit> Handle(StopSpectatingCommand request, CancellationToken cancellationToken)
    {
        if (_session.SpectateSession is null)
        {
            _logger.LogError("{Token} | Attempted to stop spectating as non-spectator", _session.Token);

            return Unit.Task;
        }

        _session.SpectateSession.Disconnect(_session);

        return Unit.Task;
    }
}
