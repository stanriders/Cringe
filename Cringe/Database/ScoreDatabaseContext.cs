using System.IO;
using Cringe.Types.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cringe.Database
{
    public sealed class ScoreDatabaseContext : DbContext
    {
        private readonly string _connectionString;

        public ScoreDatabaseContext(IConfiguration configuration)
        {
            var dbPath = configuration["DbFolder"] ?? "./";
            _connectionString = $"Filename={Path.Combine(dbPath, "scores.db")}";

            Database.Migrate();
        }

        public DbSet<SubmittedScore> Scores { get; set; }

        public DbSet<RecentScore> RecentScores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}
