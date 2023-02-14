using System.Collections.Generic;

namespace Cringe.Types.Common;

public class BaseEntity
{
    private readonly List<BaseEvent> _events = new();
    public IReadOnlyList<BaseEvent> Events => _events.AsReadOnly();

    public void AddEvent(BaseEvent @event)
    {
        _events.Add(@event);
    }

    public void ClearEvents()
    {
        _events.Clear();
    }
}
