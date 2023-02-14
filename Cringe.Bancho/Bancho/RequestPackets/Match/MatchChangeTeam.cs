using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using Cringe.Types.Enums.Multiplayer;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchChangeTeamHandler : IRequestHandler<MatchChangeTeam>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchChangeTeamHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(MatchChangeTeam request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        _lobby.Transform(matchId, match => match.ChangeTeam(_session.Id));

        return Task.CompletedTask;
    }
}

public class MatchChangeTeam : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.MatchChangeTeam;
}
