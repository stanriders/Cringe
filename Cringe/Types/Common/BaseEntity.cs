using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cringe.Types.Common;

public class BaseEntity
{
    private readonly List<BaseEvent> _events = new();
    public IReadOnlyList<BaseEvent> Events => _events.AsReadOnly();

    private readonly SemaphoreSlim _executionLock = new(1);

    public async Task Dispatch(Action<BaseEntity> action, Func<IReadOnlyList<BaseEvent>, Task> dispatch)
    {
        List<BaseEvent> _localEvents;
        await _executionLock.WaitAsync();
        try
        {
            action(this);
            _localEvents = _events.ToList();
            _events.Clear();
        }
        finally
        {
            _executionLock.Release();
        }

        try
        {
            await dispatch(_localEvents);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unhandled exception at {nameof(BaseEntity)}: {ex}");
        }
    }

    protected void AddEvent(BaseEvent @event)
    {
        _events.Add(@event);
    }
}