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

public class CreateMatchRequest : RequestPacket, IRequest
{
    [PeppyField]
    public Types.Match Match { get; set; }

    public override ClientPacketType Type => ClientPacketType.CreateMatch;
}

public class CreateMatchHandler : IRequestHandler<CreateMatchRequest>
{
    private readonly ILogger<CreateMatchHandler> _logger;
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public CreateMatchHandler(ILogger<CreateMatchHandler> logger, CurrentPlayerProvider currentPlayerProvider,
        LobbyService lobby)
    {
        _logger = logger;
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public async Task Handle(CreateMatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var match = await _lobby.CreateMatch(request.Match);
            await _lobby.JoinMatch(_session.Id, match.Id, match.Password);

            _session.Queue.EnqueuePacket(new MatchJoinSuccess(match));
            _session.Queue.EnqueuePacket(new ChannelJoinSuccess(GlobalChat.Multiplayer));
            _session.Queue.EnqueuePacket(new ResponsePackets.Match.MatchTransferHost());
        }
        catch (Exception)
        {
            _session.Queue.EnqueuePacket(new MatchJoinFail());

            throw;
        }
    }
}
