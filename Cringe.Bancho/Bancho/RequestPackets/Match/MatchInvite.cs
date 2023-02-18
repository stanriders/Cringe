using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchInviteHandler : IRequestHandler<MatchInvite>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchInviteHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(MatchInvite request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        var (matchName, matchPassword) = _lobby.GetValue(matchId, x => (x.RoomName, x.Password));

        var user = PlayersPool.GetPlayer(request.ReceiverId);
        if (user is null)
        {
            _session.Queue.EnqueuePacket(new Notification("User is not online"));

            return Task.CompletedTask;
        }

        var embed = $"Zahodi pojugama: [osump://{matchId}/{matchPassword} {matchName}]";
        user.Queue.EnqueuePacket(
            new ResponsePackets.Match.MatchInvite(_session.Player, user.Player.Username, embed));

        return Task.CompletedTask;
    }
}

public class MatchInvite : RequestPacket, IRequest
{
    [PeppyField]
    public int ReceiverId { get; init; }

    public override ClientPacketType Type => ClientPacketType.MatchInvite;
}
