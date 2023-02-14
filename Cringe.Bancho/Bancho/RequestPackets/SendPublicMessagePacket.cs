using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cringe.Bancho.Bancho.RequestPackets;

public class SendPublicMessageRequest : RequestPacket, IRequest
{
    [PeppyField]
    public string Idk { get; init; }

    [PeppyField]
    public string Text { get; init; }

    [PeppyField]
    public string Receiver { get; init; }

    public override ClientPacketType Type => ClientPacketType.SendPublicMessage;
}

public class SendPublicMessageHandler : IRequestHandler<SendPublicMessageRequest>
{
    private readonly ILogger<SendPublicMessageHandler> _logger;
    private readonly PlayerSession _session;

    public SendPublicMessageHandler(ILogger<SendPublicMessageHandler> logger,
        CurrentPlayerProvider currentPlayerProvider)
    {
        _logger = logger;
        _session = currentPlayerProvider.Session;
    }

    public Task<Unit> Handle(SendPublicMessageRequest request, CancellationToken cancellationToken)
    {
        var message = new Message(request.Text, _session.Player, request.Receiver);
        _logger.LogInformation("{Token} | Sends message {Message}", _session.Token, message);
        if (message.Receiver == GlobalChat.Multiplayer.Name)
        {
            /*
            //TODO: fix
            if (_session.MatchSession is not null)
                foreach (var slot in _session.MatchSession.Match.Slots)
                {
                }
            // slot.Player?.ReceiveMessage(message);
            else
                _logger.LogError("{Token} | Sends message to #multiplayer while his MatchSession is null",
                    _session.Token);
                    */
        }
        else if (message.Receiver == GlobalChat.Spectate.Name)
        {
            if (_session.SpectateSession is not null)
            {
                foreach (var viewer in _session.SpectateSession.Viewers.Values)
                    viewer.ReceiveMessage(message);
                _session.SpectateSession.Host.ReceiveMessage(message);
            }
            else
                _logger.LogError("{Token} | Sends message to #spectator while his SpectateSession is null",
                    _session.Token);
        }

        if (!ChatService.SendGlobalMessage(message))
            _logger.LogWarning("{Token} | User tries to send a message {Message}", _session.Token, message);

        return Unit.Task;
    }
}
