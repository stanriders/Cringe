using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Match;
using Cringe.Bancho.Services;
using Cringe.Types.Common;
using MediatR;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchFailedEvent(int[] Players, int SlotId) : BaseEvent;

public class NotifyPlayersOnMatchFailed : INotificationHandler<MatchFailedEvent>
{
    public Task Handle(MatchFailedEvent notification, CancellationToken cancellationToken)
    {
        PlayersPool.Notify(notification.Players, new MatchPlayerFailed(notification.SlotId));

        return Task.CompletedTask;
    }
}
