using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Bancho.Bancho.ResponsePackets.Match;
using Cringe.Bancho.Services;
using Cringe.Bancho.Types;
using Cringe.Types.Common;
using MediatR;

namespace Cringe.Bancho.Events.Multiplayer;

public record MatchStartEvent(List<int> Players, Match Match) : BaseEvent;

public class NotifyPlayersOnMatchStart : INotificationHandler<MatchStartEvent>
{
    public Task Handle(MatchStartEvent notification, CancellationToken cancellationToken)
    {
        PlayersPool.Notify(notification.Players, new MatchStart(notification.Match));

        return Task.CompletedTask;
    }
}
