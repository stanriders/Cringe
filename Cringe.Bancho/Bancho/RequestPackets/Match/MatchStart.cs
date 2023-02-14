using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchStartHandler : IRequestHandler<MatchStart>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchStartHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(MatchStart request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        _lobby.Transform(matchId, x => x.Start(_session.Id));

        return Task.CompletedTask;
    }
}

public class MatchStart : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.MatchStart;
}
