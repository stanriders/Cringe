using Cringe.Types.Database;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Database
{
    public sealed class PlayerDatabaseContext : DbContext
    {
        private const string connection_string = "Filename=./players.db";

        public PlayerDatabaseContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Player> Players { get; set; }

        public DbSet<PlayerRankQuery> PlayerRankQuery { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connection_string);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerRankQuery>(eb => { eb.HasNoKey(); });
        }
    }
}