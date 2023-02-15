using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Services;
using Cringe.Types.Common;
using MediatR;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchLoadComplete(List<int> Players) : BaseEvent;

public class NotifyPlayersOnMatchLoadComplete : INotificationHandler<MatchLoadComplete>
{
    public Task Handle(MatchLoadComplete notification, CancellationToken cancellationToken)
    {
        PlayersPool.Notify(notification.Players, new Bancho.ResponsePackets.Match.MatchLoadComplete());

        return Task.CompletedTask;
    }
}
