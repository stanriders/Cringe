using System;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Match;
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

    public Task<Unit> Handle(MatchSkipRequest request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        lock (_lock)
        {
            _lobby.Transform(matchId, x => x.Skip(_session.Id));
        }

        return Unit.Task;
    }
}

public class MatchSkipRequest : RequestPacket, IRequest<Unit>
{
    public override ClientPacketType Type => ClientPacketType.MatchSkipRequest;
}
