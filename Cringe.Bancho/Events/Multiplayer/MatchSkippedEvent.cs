using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Match;
using Cringe.Bancho.Services;
using Cringe.Types.Common;
using MediatR;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchSkippedEvent(List<int> Players) : BaseEvent;

public class NotifyPlayersOnMatchSkipped : INotificationHandler<MatchSkippedEvent>
{
    public Task Handle(MatchSkippedEvent notification, CancellationToken cancellationToken)
    {
        PlayersPool.Notify(notification.Players, new MatchSkip());

        return Task.CompletedTask;
    }
}
