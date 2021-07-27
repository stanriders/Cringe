using System.IO;
using Cringe.Types.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cringe.Database
{
    public sealed class BeatmapDatabaseContext : DbContext
    {
        private readonly string _connectionString;

        public BeatmapDatabaseContext(IConfiguration configuration)
        {
            var dbPath = configuration["DbFolder"] ?? "./";
            _connectionString = $"Filename={Path.Combine(dbPath, "beatmaps.db")}";

            Database.EnsureCreated();
        }

        public DbSet<Beatmap> Beatmaps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}
