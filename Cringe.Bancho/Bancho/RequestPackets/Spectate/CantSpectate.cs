using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Spectate;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Spectate;

public class CantSpectateRequest : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.CantSpectate;
}

public class CantSpectateHandler : IRequestHandler<CantSpectateRequest>
{
    private readonly ILogger<CantSpectateHandler> _logger;
    private readonly PlayerSession _session;

    public CantSpectateHandler(ILogger<CantSpectateHandler> logger, CurrentPlayerProvider currentPlayerProvider)
    {
        _logger = logger;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(CantSpectateRequest request, CancellationToken cancellationToken)
    {
        if (_session.SpectateSession is null)
        {
            _logger.LogError("{Token} | Can't spectate... foryle", _session.Token);

            return Task.CompletedTask;
        }

        var packet = new SpectatorCantSpectate(_session.Id);
        _session.SpectateSession.Host.Queue.EnqueuePacket(packet);

        foreach (var viewer in _session.SpectateSession.Viewers.Values)
        {
            viewer.Queue.EnqueuePacket(packet);
        }

        return Task.CompletedTask;
    }
}
