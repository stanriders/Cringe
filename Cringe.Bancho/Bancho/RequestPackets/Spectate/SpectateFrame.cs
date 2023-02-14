using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Spectate;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Spectate;

public class SpectateFrameRequest : PeppyRawPayload, IRequest
{
    public override ClientPacketType Type => ClientPacketType.SpectateFrames;
}

public class SpectateFrameHandler : IRequestHandler<SpectateFrameRequest>
{
    private readonly ILogger<SpectateFrameHandler> _logger;
    private readonly PlayerSession _session;

    public SpectateFrameHandler(ILogger<SpectateFrameHandler> logger, CurrentPlayerProvider currentPlayerProvider)
    {
        _logger = logger;
        _session = currentPlayerProvider.Session;
    }

    public Task<Unit> Handle(SpectateFrameRequest request, CancellationToken cancellationToken)
    {
        var spec = _session.SpectateSession;

        if (spec is null)
        {
            _logger.LogError("{Token} | SpectateSession is null and SpectateFrame is invoked", _session.Token);

            return Unit.Task;
        }

        if (spec.Host.Id != _session.Id) return Unit.Task;

        var frame = new SpectateFrames(request.Payload);
        foreach (var viewer in spec.Viewers.Values)
            viewer.Queue.EnqueuePacket(frame);

        return Unit.Task;
    }
}
