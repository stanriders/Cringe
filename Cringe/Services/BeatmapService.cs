using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Types.Database;
using Cringe.Types.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cringe.Services
{
    public class BeatmapService
    {
        private readonly OsuApiWrapper _apiService;
        private readonly IConfiguration _configuration;
        private readonly BeatmapDatabaseContext _dbContext;

        private readonly ILogger<BeatmapService> _logger;

        private bool _seeded = false;

        public BeatmapService(OsuApiWrapper apiService, IConfiguration configuration, BeatmapDatabaseContext dbContext, ILogger<BeatmapService> logger)
        {
            _apiService = apiService;
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<byte[]> GetBeatmapBytes(int beatmapId)
        {
            var cachePath = _configuration["BeatmapCachePath"];
            var beatmapPath = $"{cachePath}/{beatmapId}.osu";

            if (!File.Exists(beatmapPath))
                await _apiService.DownloadBeatmap(beatmapId);

            return await File.ReadAllBytesAsync(beatmapPath);
        }

        #region Beatmap database seeding

        public void SeedDatabse()
        {
            if (_seeded)
            {
                _logger.LogInformation("Can't seed the database - it has already been seeded in this instance");
                return;
            }

            var cachePath = _configuration["BeatmapCachePath"];

            _logger.LogInformation($"Started seeding {Directory.EnumerateFiles(cachePath, "*.osu").Count()} maps...");

            var addedMaps = 0;

            foreach (var beatmapPath in Directory.EnumerateFiles(cachePath, "*.osu").AsParallel())
            {
                try
                {
                    if (addedMaps % 10000 == 0)
                        _dbContext.SaveChanges();

                    var firstLine = File.ReadLines(beatmapPath).FirstOrDefault();

                    if (string.IsNullOrEmpty(firstLine) || !firstLine.StartsWith("osu file format"))
                        continue;

                    var beatmapModel = ParseBeatmap(beatmapPath);

                    // some people just want everything to break (see /b/2269460)
                    var nans = beatmapModel.GetType()
                        .GetProperties()
                        .Where(x => x.PropertyType == typeof(double) && double.IsNaN((double) x.GetValue(beatmapModel)))
                        .Select(x => x.Name).ToArray();

                    if (nans.Length > 0)
                    {
                        _logger.LogInformation($"Map {beatmapPath} contains NaN on fields: {string.Join(' ', nans)}");

                        continue;
                    }

                    using var md5 = MD5.Create();
                    using var stream = File.OpenRead(beatmapPath);

                    beatmapModel.Md5 = BitConverter.ToString(md5.ComputeHash(stream))
                        .Replace("-", string.Empty)
                        .ToLowerInvariant();

                    _dbContext.Beatmaps.Add(beatmapModel);
                    Interlocked.Add(ref addedMaps, 1);
                }
                catch (DbUpdateException ioEx)
                {
                    _logger.LogError(ioEx, "Failed to save");

                    return;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Beatmap seeding failed for {Beatmap}", beatmapPath);
                }
            }

            _dbContext.SaveChanges();
            _logger.LogInformation($"Beatmap seeding finished! Added {addedMaps} maps");

            _seeded = true;
        }

        private Beatmap ParseBeatmap(string beatmapPath)
        {
            int.TryParse(Path.GetFileNameWithoutExtension(beatmapPath), out var beatmapId);

            var beatmap = new Beatmap
            {
                Id = beatmapId,
                Status = RankedStatus.Ranked
            };

            var parsingTimingPoints = false;
            var parsingHitObjects = false;

            var firstHitObjectTime = 0;
            var previousHitObjectTime = 0;

            foreach (var line in File.ReadLines(beatmapPath))
            {
                if (parsingTimingPoints)
                {
                    var timingPoint = line.Split(',');
                    if (timingPoint.Length > 1)
                    {
                        var beatLength = double.Parse(timingPoint[1]);
                        if (beatLength > 0)
                        {
                            beatmap.Bpm = Math.Round(60000.0 / beatLength, 2);
                            if (double.IsNaN(beatmap.Bpm))
                                beatmap.Bpm = -1;

                            parsingTimingPoints = false;
                        }
                    }
                }

                if (parsingHitObjects)
                {
                    var hitObject = line.Split(',');
                    if (hitObject.Length > 2)
                    {
                        var hitObjectTime = int.Parse(hitObject[2]);
                        if (previousHitObjectTime == 0)
                            firstHitObjectTime = hitObjectTime;

                        previousHitObjectTime = hitObjectTime;
                    }
                }

                if (line.StartsWith("Mode:"))
                    beatmap.Mode = (GameModes) int.Parse(line[^1..]);
                else if (line.StartsWith("Title:"))
                    beatmap.Title = line[6..];
                else if (line.StartsWith("Artist:"))
                    beatmap.Artist = line[7..];
                else if (line.StartsWith("Creator:"))
                    beatmap.Creator = line[8..];
                else if (line.StartsWith("Version:"))
                    beatmap.DifficultyName = line[8..];
                else if (line.StartsWith("BeatmapID:") && beatmapId == default)
                    beatmap.Id = int.Parse(line[10..]);
                else if (line.StartsWith("BeatmapSetID:"))
                    beatmap.BeatmapSetId = int.Parse(line[13..]);
                else if (line.StartsWith("HPDrainRate:"))
                    beatmap.HpDrain = double.Parse(line[12..]);
                else if (line.StartsWith("CircleSize:"))
                    beatmap.CircleSize = double.Parse(line[11..]);
                else if (line.StartsWith("OverallDifficulty:"))
                    beatmap.OverallDifficulty = double.Parse(line[18..]);
                else if (line.StartsWith("ApproachRate:"))
                    beatmap.ApproachRate = double.Parse(line[13..]);
                else if (line.StartsWith("[TimingPoints]"))
                    parsingTimingPoints = true;
                else if (line.StartsWith("[HitObjects]"))
                    parsingHitObjects = true;
            }

            if (parsingHitObjects)
                beatmap.Length = previousHitObjectTime - firstHitObjectTime;

            // very old beatmaps have ar == od
            if (beatmap.ApproachRate == 0 && beatmapId < 67000)
                beatmap.ApproachRate = beatmap.OverallDifficulty;

            return beatmap;
        }

        #endregion
    }
}
