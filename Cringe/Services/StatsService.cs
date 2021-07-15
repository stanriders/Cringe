using System.Collections.Generic;
using Cringe.Types;
using Microsoft.Extensions.Logging;

namespace Cringe.Services
{
    public class StatsService
    {
        private readonly ILogger<StatsService> _logger;
        private readonly Dictionary<int, Stats> _stats = new();

        public StatsService(ILogger<StatsService> logger)
        {
            _logger = logger;
        }

        public Stats GetUpdates(int id)
        {
            if (!_stats.TryGetValue(id, out var value)) return null;

            _stats.Remove(id);
            return value;
        }

        public void SetUpdates(int id, Stats newStats)
        {
            if (!_stats.TryAdd(id, newStats))
                _stats[id] = newStats;
        }
    }
}