using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchChangeModsHandler : IRequestHandler<MatchChangeMods>
{
    private readonly LobbyService _lobby;
    private readonly ILogger<MatchChangeModsHandler> _logger;
    private readonly PlayerSession _session;

    public MatchChangeModsHandler(LobbyService lobby, ILogger<MatchChangeModsHandler> logger,
        CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _logger = logger;
        _session = currentPlayerProvider.Session;
    }

    public Task<Unit> Handle(MatchChangeMods request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        _lobby.Transform(matchId, match => match.SetMods(_session.Id, request.Mods));

        return Unit.Task;
    }
}

public class MatchChangeMods : RequestPacket, IRequest
{
    [PeppyField]
    [EnumType(typeof(int))]
    public Mods Mods { get; init; }

    public override ClientPacketType Type => ClientPacketType.MatchChangeMods;
}
