using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Match;
using Cringe.Bancho.Services;
using Cringe.Types.Common;
using MediatR;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchPlayerSkippedEvent(List<int> Players, int SlotId) : BaseEvent;

public class NotifyPlayersOnMatchPlayerSkipped : INotificationHandler<MatchPlayerSkippedEvent>
{
    public Task Handle(MatchPlayerSkippedEvent notification, CancellationToken cancellationToken)
    {
        PlayersPool.Notify(notification.Players, new MatchPlayerSkipped(notification.SlotId));

        return Task.CompletedTask;
    }
}
