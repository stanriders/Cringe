using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class ChannelPartRequest : RequestPacket, IRequest
{
    [PeppyField]
    public string Chat { get; init; }

    public override ClientPacketType Type => ClientPacketType.ChannelPart;
}

public class ChannelPartHandler : IRequestHandler<ChannelPartRequest>
{
    private readonly ILogger<ChannelPartHandler> _logger;
    private readonly PlayerSession _session;

    public ChannelPartHandler(ILogger<ChannelPartHandler> logger, CurrentPlayerProvider currentPlayerProvider)
    {
        _logger = logger;
        _session = currentPlayerProvider.Session;
    }

    public Task<Unit> Handle(ChannelPartRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Token} | Leaves the {Chat} chat", _session.Token, request.Chat);
        ChatService.GetChat(request.Chat)?.Disconnect(_session);

        return Unit.Task;
    }
}
