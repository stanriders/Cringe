using System;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchScoreUpdateHandler : IRequestHandler<MatchScoreUpdate>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchScoreUpdateHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(MatchScoreUpdate request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        var playerPosition = _lobby.GetValue(matchId, x => x.PlayerPosition(_session.Id));

        request.Payload[4] = (byte) playerPosition;
        var packet = new ResponsePackets.Match.MatchScoreUpdate(request.Payload);
        //TODO: dispatching
        /*
        foreach (var player in session.MatchSession.Match.PlayingPlayers)
            player.Player.Queue.EnqueuePacket(packet);
            */

        return Task.CompletedTask;
    }
}

public class MatchScoreUpdate : PeppyRawPayload, IRequest
{
    public override ClientPacketType Type => ClientPacketType.MatchScoreUpdate;
}
