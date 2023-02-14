using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class ChangeActionRequest : RequestPacket, IRequest
{
    [PeppyField]
    public ChangeAction Action { get; init; }

    public override ClientPacketType Type => ClientPacketType.ChangeAction;
}

public class ChangeActionHandler : IRequestHandler<ChangeActionRequest>
{
    private readonly StatsService _stats;
    private readonly ILogger<ChangeActionHandler> _logger;
    private readonly PlayerSession _session;

    public ChangeActionHandler(StatsService stats,
        ILogger<ChangeActionHandler> logger,
        CurrentPlayerProvider currentPlayerProvider)
    {
        _stats = stats;
        _logger = logger;
        _session = currentPlayerProvider.Session;
    }

    public async Task Handle(ChangeActionRequest request, CancellationToken cancellationToken)
    {
        var stats = await _stats.GetUpdates(_session.Id);
        stats.Action = request.Action;

        _logger.LogInformation("{Token} | Changes action to {@Action}", _session.Token, stats.Action);

        _stats.SetUpdates(_session.Id, stats);
        _session.Queue.EnqueuePacket(new UserStats(stats));
        _session.Queue.EnqueuePacket(new UserPresence(_session.Presence));
    }
}
