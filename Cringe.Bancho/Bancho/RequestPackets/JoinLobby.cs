using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class JoinLobbyRequest : RequestPacket, IRequest
{
    public override ClientPacketType Type => ClientPacketType.JoinLobby;
}

public class JoinLobbyHandler : IRequestHandler<JoinLobbyRequest>
{
    private readonly ILogger<JoinLobbyHandler> _logger;
    private readonly LobbyService _lobby;
    private readonly PlayerSession _session;

    public JoinLobbyHandler(CurrentPlayerProvider currentPlayerProvider, ILogger<JoinLobbyHandler> logger,
        LobbyService lobby)
    {
        _logger = logger;
        _lobby = lobby;
        _session = currentPlayerProvider.Session;
    }

    public async Task Handle(JoinLobbyRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Token} | Logged in the lobby", _session.Token);
        ChatService.GetChat(ChatService.LobbyName)?.Connect(_session);
        await _lobby.JoinLobby(_session.Id);
    }
}
