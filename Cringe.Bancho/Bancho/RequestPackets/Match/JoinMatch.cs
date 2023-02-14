using System;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Bancho.ResponsePackets.Match;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class JoinMatchHandler : IRequestHandler<JoinMatch>
{
    private readonly LobbyService _lobby;
    private readonly ILogger<JoinMatch> _logger;
    private readonly PlayerSession _session;

    public JoinMatchHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider,
        ILogger<JoinMatch> logger)
    {
        _lobby = lobby;
        _logger = logger;
        _session = currentPlayerProvider.Session;
    }

    public Task<Unit> Handle(JoinMatch request, CancellationToken cancellationToken)
    {
        try
        {
            var match = _lobby.JoinLobby(_session.Id, (short) request.MatchId, request.Password);

            _session.Queue.EnqueuePacket(new MatchJoinSuccess(match));
            _session.Queue.EnqueuePacket(new ChannelJoinSuccess(GlobalChat.Multiplayer));

            return Unit.Task;
        }
        catch (Exception)
        {
            _session.Queue.EnqueuePacket(new MatchJoinFail());

            throw;
        }
    }
}

public class JoinMatch : RequestPacket, IRequest
{
    [PeppyField]
    public int MatchId { get; init; }

    [PeppyField]
    public string Password { get; init; }

    public override ClientPacketType Type => ClientPacketType.JoinMatch;
}
