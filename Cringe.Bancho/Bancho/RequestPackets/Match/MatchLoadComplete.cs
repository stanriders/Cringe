using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchLoadCompleteHandler : IRequestHandler<MatchLoadComplete>
{
    private readonly object _key = new();
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchLoadCompleteHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(MatchLoadComplete request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);

        lock (_key)
        {
            _lobby.Transform(matchId, x => x.LoadComplete(_session.Id));
        }

        return Task.CompletedTask;
    }
}

public class MatchLoadComplete : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.MatchLoadComplete;
}
