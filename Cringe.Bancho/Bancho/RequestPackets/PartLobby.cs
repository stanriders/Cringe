using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class PartLobbyRequest : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.PartLobby;
}

public class PartLobby : IRequestHandler<PartLobbyRequest>
{
    private readonly ILogger<PartLobby> _logger;
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public PartLobby(CurrentPlayerProvider currentPlayerProvider, ILogger<PartLobby> logger, LobbyService lobby)
    {
        _logger = logger;
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(PartLobbyRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Token} | Logged out the lobby", _session.Token);
        ChatService.GetChat(ChatService.LobbyName)?.Disconnect(_session);
        _lobby.LeaveLobby(_session.Id);

        return Task.CompletedTask;
    }
}
