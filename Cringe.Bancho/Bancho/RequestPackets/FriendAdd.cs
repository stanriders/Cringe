using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class FriendAddRequest : RequestPacket, IRequest
{
    [PeppyField]
    public int FriendId { get; init; }

    public override ClientPacketType Type => ClientPacketType.FriendAdd;
}

public class FriendAddHandler : IRequestHandler<FriendAddRequest>
{
    private readonly ILogger<FriendAddHandler> _logger;
    private readonly FriendsService _friendsService;
    private readonly PlayerSession _session;

    public FriendAddHandler(ILogger<FriendAddHandler> logger, FriendsService friendsService,
        CurrentPlayerProvider currentPlayerProvider)
    {
        _logger = logger;
        _friendsService = friendsService;
        _session = currentPlayerProvider.Session;
    }


    public async Task<Unit> Handle(FriendAddRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Token} | Adding friend {Friend}...", _session.Token, request.FriendId);

        await _friendsService.AddFriend(_session.Player.Id, request.FriendId);

        return Unit.Value;
    }
}
