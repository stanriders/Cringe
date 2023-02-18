using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class FriendRemoveRequest : RequestPacket, IRequest
{
    [PeppyField]
    public int FriendId { get; init; }

    public override ClientPacketType Type => ClientPacketType.FriendRemove;
}

public class FriendRemoveHandler : IRequestHandler<FriendRemoveRequest>
{
    private readonly ILogger<FriendRemoveHandler> _logger;
    private readonly FriendsService _friendsService;
    private readonly PlayerSession _session;

    public FriendRemoveHandler(CurrentPlayerProvider currentPlayerProvider, ILogger<FriendRemoveHandler> logger,
        FriendsService friendsService)
    {
        _logger = logger;
        _friendsService = friendsService;
        _session = currentPlayerProvider.Session;
    }

    public async Task Handle(FriendRemoveRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Token} | Removing friend {Friend}...", _session.Token, request.FriendId);

        await _friendsService.RemoveFriend(_session.Id, request.FriendId);
    }
}
