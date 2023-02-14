using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchTransferHostHandler : IRequestHandler<MatchTransferHost>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchTransferHostHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(MatchTransferHost request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        _lobby.Transform(matchId, x => x.TransferHost(_session.Id, request.SlotId));

        return Task.CompletedTask;
    }
}

public class MatchTransferHost : RequestPacket, IRequest
{
    [PeppyField]
    public int SlotId { get; set; }

    public override ClientPacketType Type => ClientPacketType.MatchTransferHost;
}
