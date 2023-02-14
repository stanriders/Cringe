using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchChangeSettingsHandler : IRequestHandler<MatchChangeSettings>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchChangeSettingsHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(MatchChangeSettings request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        _lobby.Transform(matchId, match => match.ChangeSettings(_session.Id, request.Match));

        return Task.CompletedTask;
    }
}

public class MatchChangeSettings : RequestPacket, IRequest
{
    [PeppyField]
    public Types.Match Match { get; init; }

    public override ClientPacketType Type => ClientPacketType.MatchChangeSettings;
}
