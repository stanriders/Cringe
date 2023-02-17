using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchNoBeatmapHandler : IRequestHandler<MatchNoBeatmap>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchNoBeatmapHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public async Task Handle(MatchNoBeatmap request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        await _lobby.Transform(matchId, x => x.NoBeatmap(_session.Id));
    }
}

public class MatchNoBeatmap : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.MatchNoBeatmap;
}
