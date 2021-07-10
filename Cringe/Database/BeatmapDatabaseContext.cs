using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Cringe.Types;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Database
{
    public sealed class BeatmapDatabaseContext : DbContext
    {
        private const string connection_string = "Filename=./beatmaps.db";

        public BeatmapDatabaseContext()
        {
            if (Database.EnsureCreated())
                // TODO: only seed when requested
                SeedDatabase();
        }

        public DbSet<Beatmap> Beatmaps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connection_string);
        }

        private void SeedDatabase()
        {
            // TODO: better config
            var cachePath = Startup.Configuration["BeatmapCachePath"];

            var addedMaps = 0;

            foreach (var beatmapFile in Directory.EnumerateFiles(cachePath, "*.osu").AsParallel())
                try
                {
                    if (addedMaps % 10000 == 0)
                        SaveChanges();

                    if (!int.TryParse(Path.GetFileNameWithoutExtension(beatmapFile), out var beatmapId))
                        continue;

                    var beatmapModel = new Beatmap
                    {
                        Id = beatmapId,
                        Ranked = true
                    };

                    using var md5 = MD5.Create();
                    using var stream = File.OpenRead(beatmapFile);

                    beatmapModel.Md5 = BitConverter.ToString(md5.ComputeHash(stream))
                        .Replace("-", string.Empty)
                        .ToLowerInvariant();

                    Beatmaps.Add(beatmapModel);
                    Interlocked.Add(ref addedMaps, 1);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Beatmap seeding failed for {beatmapFile}: {e}");
                }

            SaveChanges();
        }
    }
}