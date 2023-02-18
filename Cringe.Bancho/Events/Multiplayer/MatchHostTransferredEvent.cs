using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Match;
using Cringe.Bancho.Services;
using Cringe.Types.Common;
using MediatR;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchHostTransferredEvent(List<int> Players) : BaseEvent;

public class NotifyPlayersOnMatchHostTransfer : INotificationHandler<MatchHostTransferredEvent>
{
    public Task Handle(MatchHostTransferredEvent notification, CancellationToken cancellationToken)
    {
        PlayersPool.Notify(notification.Players, new MatchTransferHost());

        return Task.CompletedTask;
    }
}
