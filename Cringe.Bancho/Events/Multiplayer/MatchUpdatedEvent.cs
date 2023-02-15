using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Match;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Common;
using MediatR;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchUpdatedEvent(Match Match) : BaseEvent;

public record NotifyPlayersOnMatchUpdate : INotificationHandler<MatchUpdatedEvent>
{
    public Task Handle(MatchUpdatedEvent notification, CancellationToken cancellationToken)
    {
        PlayersPool.Notify(notification.Match.Players, new UpdateMatch(notification.Match));

        return Task.CompletedTask;
    }
}
