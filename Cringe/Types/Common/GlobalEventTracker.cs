using System.Collections.Generic;
using System.Threading;

namespace Cringe.Types.Common;

//TODO: probably not a good idea
public static class GlobalEventTracker
{
    public static readonly SemaphoreSlim Process = new(1);
    private static readonly List<BaseEvent> _events = new();
    public static IEnumerable<BaseEvent> Events => _events.AsReadOnly();

    public static void AddEvent(BaseEvent @event)
    {
        Process.Wait();
        _events.Add(@event);
        Process.Release();
    }

    public static void ClearEvents()
    {
        _events.Clear();
    }
}
