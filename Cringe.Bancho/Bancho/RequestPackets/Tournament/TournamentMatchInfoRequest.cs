using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;

namespace Cringe.Bancho.Bancho.RequestPackets.Tournament;

public class TournamentMatchInfoRequest : RequestPacket, IRequest
{
    [PeppyField]
    public int TournamentId { get; set; }

    public override ClientPacketType Type => ClientPacketType.TournamentMatchInfoRequest;
}

public class TournamentMatchInfoHandler : IRequestHandler<TournamentMatchInfoRequest>
{
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public TournamentMatchInfoHandler(LobbyService lobby, CurrentPlayerProvider currentPlayerProvider)
    {
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(TournamentMatchInfoRequest request, CancellationToken cancellationToken)
    {
        var lobby = _lobby.GetValue((short) request.TournamentId, x => x);

        if (lobby is null)
        {
            _session.Queue.EnqueuePacket(new Notification("Lobbeshnik zakonchilsya"));

            return Task.CompletedTask;
        }


        _session.UpdateMatch(lobby);

        return Task.CompletedTask;
    }
}
