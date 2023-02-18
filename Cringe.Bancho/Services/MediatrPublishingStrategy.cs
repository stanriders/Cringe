using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Types.Common;
using MediatR;

namespace Cringe.Bancho.Services;

public class MediatrPublishingStrategy : IPublishingStrategy
{
    private readonly IMediator _mediator;

    public MediatrPublishingStrategy(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Publish(BaseEvent @event, CancellationToken token = default)
    {
        await _mediator.Publish(@event, token);
    }

    public async Task Publish(IEnumerable<BaseEvent> events, CancellationToken token = default)
    {
        foreach (var ev in events)
        {
            await Publish(ev, token);
        }
    }
}
