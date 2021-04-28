using Microsoft.EntityFrameworkCore;
using osuLocalBancho.Types;

namespace osuLocalBancho.Database
{
    public sealed class ScoreDatabaseContext : DbContext
    {
        private const string connection_string = "Filename=./scores.db";

        public ScoreDatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connection_string);
        }

        public DbSet<SubmittedScore> Scores { get; set; }
    }
}
