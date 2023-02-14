using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchChangeSlotHandler : IRequestHandler<MatchChangeSlot>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchChangeSlotHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(MatchChangeSlot request, CancellationToken cancellationToken)
    {
        var match = _lobby.FindMatch(_session.Id);
        _lobby.Transform(match, x => x.ChangeSlot(_session.Id, request.SlotId));

        return Task.CompletedTask;
    }
}

public class MatchChangeSlot : RequestPacket, IRequest
{
    [PeppyField]
    public int SlotId { get; set; }

    public override ClientPacketType Type => ClientPacketType.MatchChangeSlot;
}
