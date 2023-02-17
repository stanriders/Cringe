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
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchFailedHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public async Task Handle(MatchFailed request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        await _lobby.Transform(matchId, x => x.Failed(_session.Id));
    }
}

public class MatchFailed : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.MatchFailed;
}
