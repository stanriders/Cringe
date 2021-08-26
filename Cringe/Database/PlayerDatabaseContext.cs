using System.IO;
using Cringe.Types.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cringe.Database
{
    public sealed class PlayerDatabaseContext : DbContext
    {
        private readonly string _connectionString;

        public PlayerDatabaseContext(IConfiguration configuration)
        {
            var dbPath = configuration["DbFolder"] ?? "./";
            _connectionString = $"Filename={Path.Combine(dbPath, "players.db")}";

            Database.Migrate();
        }

        public DbSet<Player> Players { get; set; }

        public DbSet<Bot> Bots { get; set; }

        public DbSet<PlayerRankQuery> PlayerRankQuery { get; set; }

        public DbSet<Friends> Friends { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerRankQuery>(entity => { entity.HasNoKey().ToView(null); });
        }
    }
}
