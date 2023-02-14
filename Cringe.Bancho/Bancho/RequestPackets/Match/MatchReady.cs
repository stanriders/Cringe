using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchReadyHandler : IRequestHandler<MatchReady>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchReadyHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(MatchReady request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        _lobby.Transform(matchId, x => x.Ready(_session.Id));

        return Task.CompletedTask;
    }
}

public class MatchReady : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.MatchReady;
}
