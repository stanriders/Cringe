using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class UserStatsRequest : RequestPacket, IRequest
{
    [PeppyField]
    public int[] Users { get; init; }

    public override ClientPacketType Type => ClientPacketType.UserStatsRequest;
}

public class UserStatsRequestHandler : IRequestHandler<UserStatsRequest>
{
    private readonly ILogger<UserStatsRequest> _logger;
    private readonly StatsService _stats;
    private readonly PlayerSession _session;

    public UserStatsRequestHandler(ILogger<UserStatsRequest> logger, CurrentPlayerProvider currentPlayerProvider,
        StatsService stats)
    {
        _logger = logger;
        _stats = stats;
        _session = currentPlayerProvider.Session;
    }

    public async Task<Unit> Handle(UserStatsRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Token} | Receive stats for these players: {Ids}", _session.Token,
            string.Join(",", request.Users));

        foreach (var playerId in request.Users)
        {
            if (playerId == _session.Id) continue;

            var stats = await _stats.GetUpdates(playerId);

            if (stats is null) continue;

            _session.Queue.EnqueuePacket(new UserStats(stats));
        }

        return Unit.Value;
    }
}
