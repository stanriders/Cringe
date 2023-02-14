using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchSkipHandler : IRequestHandler<MatchSkipRequest>
{
    private static readonly object _lock = new();
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchSkipHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(MatchSkipRequest request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        lock (_lock)
        {
            _lobby.Transform(matchId, x => x.Skip(_session.Id));
        }

        return Task.CompletedTask;
    }
}

public class MatchSkipRequest : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.MatchSkipRequest;
}
