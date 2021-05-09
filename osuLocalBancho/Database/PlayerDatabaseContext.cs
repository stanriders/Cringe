
using Microsoft.EntityFrameworkCore;
using osuLocalBancho.Types;

namespace osuLocalBancho.Database
{
    public class PlayerDatabaseContext : DbContext
    {
        private const string connection_string = "Filename=./players.db";

        public PlayerDatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connection_string);
        }

        public DbSet<Player> Players { get; set; }
    }
}
