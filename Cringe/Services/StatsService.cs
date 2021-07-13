﻿using System.Collections.Generic;
using Cringe.Types;
using Microsoft.Extensions.Logging;

namespace Cringe.Services
{
    public class StatsService
    {
        private readonly ILogger<StatsService> _logger;
        private Dictionary<int, Stats> _stats = new();

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
    }
}