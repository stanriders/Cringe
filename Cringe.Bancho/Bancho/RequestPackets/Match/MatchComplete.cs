using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchCompleteHandler : IRequestHandler<MatchComplete>
{
    private static readonly object _lock = new();
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchCompleteHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task<Unit> Handle(MatchComplete request, CancellationToken cancellationToken)
    {
        var match = _lobby.FindMatch(_session.Id);
        lock (_lock)
        {
            _lobby.Transform(match, x => x.Complete(_session.Id));
        }

        return Unit.Task;
    }
}

public class MatchComplete : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.MatchComplete;
}
