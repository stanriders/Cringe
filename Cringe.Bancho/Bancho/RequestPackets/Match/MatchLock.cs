using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchLockHandler : IRequestHandler<MatchLock>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchLockHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public async Task Handle(MatchLock request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        await _lobby.Transform(matchId, x => x.LockSlot(_session.Id, request.SlotId));
    }
}

public class MatchLock : RequestPacket, IRequest
{
    [PeppyField]
    public int SlotId { get; init; }

    public override ClientPacketType Type => ClientPacketType.MatchLock;
}
