using Cringe.Types;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Database
{
    public class PlayerDatabaseContext : DbContext
    {
        private const string connection_string = "Filename=./players.db";

        public PlayerDatabaseContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Player> Players { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connection_string);
        }
    }
}