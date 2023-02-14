using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchChangePasswordHandler : IRequestHandler<MatchChangePassword>
{
    private readonly ILogger<MatchChangeModsHandler> _logger;
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchChangePasswordHandler(ILogger<MatchChangeModsHandler> logger, LobbyService lobby,
        CurrentPlayerProvider currentPlayerProvider)
    {
        _logger = logger;
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(MatchChangePassword request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        _lobby.Transform(matchId, x => x.SetPassword(_session.Id, request.Match.Password));
        _logger.LogInformation("{Token} | Changes password for match {matchId}", _session.Token, matchId);

        return Task.CompletedTask;
    }
}

public class MatchChangePassword : RequestPacket, IRequest
{
    [PeppyField]
    public Types.Match Match { get; set; }

    public override ClientPacketType Type => ClientPacketType.MatchChangePassword;
}
