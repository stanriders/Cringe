using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class UserPresenceRequest : RequestPacket, IRequest
{
    [PeppyField]
    public int[] Users { get; init; }

    public override ClientPacketType Type => ClientPacketType.UserPresenceRequest;
}

public class UserPresenceRequestHandler : IRequestHandler<UserPresenceRequest>
{
    private readonly ILogger<UserPresenceRequestHandler> _logger;
    private readonly PlayerSession _session;

    public UserPresenceRequestHandler(
        ILogger<UserPresenceRequestHandler> logger,
        CurrentPlayerProvider currentPlayerProvider)
    {
        _logger = logger;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(UserPresenceRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Token} | Receive user presence for the players {Ids}", _session.Token,
            string.Join(",", request.Users));

        foreach (var id in request.Users)
        {
            var user = PlayersPool.GetPlayer(id);
            _session.Queue.EnqueuePacket(new UserPresence(user.Presence));
        }

        return Task.CompletedTask;
    }
}
