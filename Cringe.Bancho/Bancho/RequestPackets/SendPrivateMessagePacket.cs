using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class SendPrivateMessageRequest : RequestPacket, IRequest
{
    [PeppyField]
    public string Idk { get; init; }

    [PeppyField]
    public string Text { get; init; }

    [PeppyField]
    public string Receiver { get; init; }

    public override ClientPacketType Type => ClientPacketType.SendPrivateMessage;
}

public class SendPrivateMessageHandler : IRequestHandler<SendPrivateMessageRequest>
{
    private readonly ILogger<SendPrivateMessageHandler> _logger;
    private readonly ChatService _chat;
    private readonly PlayerSession _session;

    public SendPrivateMessageHandler(ILogger<SendPrivateMessageHandler> logger,
        CurrentPlayerProvider currentPlayerProvider, ChatService chat)
    {
        _logger = logger;
        _chat = chat;
        _session = currentPlayerProvider.Session;
    }

    public Task Handle(SendPrivateMessageRequest request, CancellationToken cancellationToken)
    {
        var message = new Message(request.Text, _session.Player, request.Receiver);
        _logger.LogInformation("{Token} | Sends the message {Message}", _session.Token, message);
        _chat.SendPrivateMessage(message);

        return Task.CompletedTask;
    }
}
