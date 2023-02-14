using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Match;

public class MatchHasBeatmapHandler : IRequestHandler<MatchHasBeatmap>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public MatchHasBeatmapHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(MatchHasBeatmap request, CancellationToken cancellationToken)
    {
        var matchId = _lobby.FindMatch(_session.Id);
        _lobby.Transform(matchId, v => v.HasBeatmap(_session.Id));

        return Task.CompletedTask;
    }
}

public class MatchHasBeatmap : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.MatchHasBeatmap;
}
