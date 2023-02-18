using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class ChannelJoinRequest : RequestPacket, IRequest
{
    [PeppyField]
    public string Chat { get; init; }

    public override ClientPacketType Type => ClientPacketType.ChannelJoin;
}

public class ChannelJoinHandler : IRequestHandler<ChannelJoinRequest>
{
    private readonly ILogger<ChannelJoinHandler> _logger;
    private readonly PlayerSession _session;

    public ChannelJoinHandler(ILogger<ChannelJoinHandler> logger, CurrentPlayerProvider currentPlayerProvider)
    {
        _logger = logger;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(ChannelJoinRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Token} | Connecting to the {Chat} chat", _session.Token, request.Chat);
        var chat = ChatService.GetChat(request.Chat);
        chat?.Connect(_session);

        return Task.CompletedTask;
    }
}
