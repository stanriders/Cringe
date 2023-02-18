using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Match;
using Cringe.Bancho.Services;
using Cringe.Types.Common;
using MediatR;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchCompletedEvent(List<int> Players) : BaseEvent;

public class MatchCompletedEventHandler : INotificationHandler<MatchCompletedEvent>
{
    public Task Handle(MatchCompletedEvent notification, CancellationToken cancellationToken)
    {
        PlayersPool.Notify(notification.Players, new MatchComplete());

        return Task.CompletedTask;
    }
}
