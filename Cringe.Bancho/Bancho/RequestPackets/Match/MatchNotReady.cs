using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchNotReadyHandler : IRequestHandler<MatchNotReady>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchNotReadyHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public async Task Handle(MatchNotReady request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        await _lobby.Transform(matchId, x => x.NotReady(_session.Id));
    }
}

public class MatchNotReady : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.MatchNotReady;
}
