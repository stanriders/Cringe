﻿
using System;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Bancho.Types;
using Cringe.Database;
using Cringe.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Cringe.Bancho.Services
{
    public class StatsService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly PlayerDatabaseContext _playerDatabaseContext;
        private readonly PlayerTopscoreStatsCache _topscoreStatsCache;

        public StatsService(IMemoryCache memoryCache, PlayerDatabaseContext playerDatabaseContext, PlayerTopscoreStatsCache topscoreStatsCache)
        {
            _memoryCache = memoryCache;
            _playerDatabaseContext = playerDatabaseContext;
            _topscoreStatsCache = topscoreStatsCache;
        }

        public async Task<Stats> GetUpdates(int id)
        {
            if (!_memoryCache.TryGetValue(id, out Stats value))
            {
                var player = PlayersPool.GetPlayer(id);
                if (player != null)
                {
                    var updatedPlayer = await _playerDatabaseContext.Players.Where(x => x.Id == id).SingleOrDefaultAsync();
                    player.Player = updatedPlayer;

                    var stats = player.Stats;
                    var topscoreStats = await _topscoreStatsCache.GetPlayerTopscoreStats(id);
                    stats.Pp = (ushort) topscoreStats.Pp;
                    stats.Accuracy = (float) (topscoreStats.Accuracy / 100.0f);

                    SetUpdates(id, stats);

                    return stats;
                }

                return null;
            }

            return value;
        }

        public void SetUpdates(int id, Stats newStats)
        {
            _memoryCache.Set(id, newStats, TimeSpan.FromMinutes(30)); // timespan is a safeguard for stuck stats
        }

        public void RemoveStats(int id)
        {
            _memoryCache.Remove(id);
        }
    }
}
