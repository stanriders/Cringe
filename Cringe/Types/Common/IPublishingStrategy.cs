using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cringe.Types.Common;

public interface IPublishingStrategy
{
    public Task Publish(BaseEvent @event, CancellationToken token = default);
    public Task Publish(IEnumerable<BaseEvent> events, CancellationToken token = default);
}
