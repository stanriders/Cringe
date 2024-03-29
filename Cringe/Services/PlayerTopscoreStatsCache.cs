﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types;
using Cringe.Types.Database;
using Cringe.Types.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Cringe.Services
{
    public class PlayerTopscoreStatsCache
    {
        private readonly IMemoryCache _memoryCache; //TODO: Maybe redis?
        private readonly ScoreDatabaseContext _scoreDatabaseContext;

        public PlayerTopscoreStatsCache(IMemoryCache memoryCache, ScoreDatabaseContext scoreDatabaseContext)
        {
            _memoryCache = memoryCache;
            _scoreDatabaseContext = scoreDatabaseContext;
        }

        public async Task<PlayerTopscoreStats> GetPlayerTopscoreStats(int playerId)
        {
            if (!_memoryCache.TryGetValue(playerId, out PlayerTopscoreStats stats))
            {
                stats = await RefreshStats(playerId);

                _memoryCache.Set(playerId, stats);
            }

            return stats;
        }

        public async Task UpdatePlayerStats(Player player)
        {
            var stats = await RefreshStats(player.Id);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (stats.Pp != player.Pp)
            {
                player.Accuracy = (float) stats.Accuracy / 100.0f;
                player.Pp = (ushort) stats.Pp;

                ClearCache(player.Id);
                _memoryCache.Set(player.Id, stats);
            }
        }

        public void ClearCache(int playerId)
        {
            _memoryCache.Remove(playerId);
        }

        private async Task<PlayerTopscoreStats> RefreshStats(int playerId)
        {
            var topScores = await _scoreDatabaseContext.Scores
                .Where(x => x.PlayerId == playerId && !x.Mods.HasFlag(Mods.Relax))
                .OrderByDescending(x => x.Pp)
                .Select(x => new {x.Pp, x.Accuracy})
                .Take(100)
                .ToArrayAsync();

            if (topScores.Length == 0)
                return new PlayerTopscoreStats();

            var index = 0;
            var pp = topScores.Sum(play => Math.Pow(0.95, index++) * play.Pp);

            var acc = topScores.Sum(play => play.Accuracy) / topScores.Length;

            return new PlayerTopscoreStats
            {
                Pp = pp,
                Accuracy = acc
            };
        }
    }
}
