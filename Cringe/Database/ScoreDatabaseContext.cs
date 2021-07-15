using Cringe.Types;
using Cringe.Types.Database;
using Microsoft.EntityFrameworkCore;

namespace Cringe.Database
{
    public sealed class ScoreDatabaseContext : DbContext
    {
        private const string connection_string = "Filename=./scores.db";

        public ScoreDatabaseContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<SubmittedScore> Scores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connection_string);
        }
    }
}