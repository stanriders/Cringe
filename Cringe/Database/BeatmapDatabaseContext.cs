using Cringe.Types.Database;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Database
{
    public sealed class BeatmapDatabaseContext : DbContext
    {
        private const string connection_string = "Filename=./beatmaps.db";

        public BeatmapDatabaseContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Beatmap> Beatmaps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connection_string);
        }
    }
}
