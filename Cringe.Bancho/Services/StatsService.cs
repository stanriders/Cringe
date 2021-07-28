using System.Collections.Concurrent;
using Cringe.Bancho.Types;

namespace Cringe.Bancho.Services
{
    public class StatsService
    {
        private readonly ConcurrentDictionary<int, Stats> _stats = new();

        public Stats GetUpdates(int id)
        {
            if (!_stats.TryGetValue(id, out var value)) return null;

            _stats.TryRemove(id, out _);

            return value;
        }

        public void SetUpdates(int id, Stats newStats)
        {
            if (!_stats.TryAdd(id, newStats))
                _stats[id] = newStats;
        }
    }
}
