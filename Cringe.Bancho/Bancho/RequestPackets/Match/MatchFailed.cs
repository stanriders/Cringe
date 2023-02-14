using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Events.Multiplayer;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchFailedHandler : IRequestHandler<MatchFailed>
{
    private readonly IMediator _mediator;
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchFailedHandler(IMediator mediator, LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _mediator = mediator;
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public async Task Handle(MatchFailed request, CancellationToken cancellationToken)
    {
        //TODO: extract it to domain events
        var matchId = _lobby.FindMatch(_session.Id);
        await _mediator.Publish(new MatchFailedEvent(matchId,
                _lobby.GetValue(matchId, v => v.PlayerPosition(_session.Id))),
            cancellationToken);
    }
}

public class MatchFailed : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.MatchFailed;
}
