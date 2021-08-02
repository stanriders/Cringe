using Microsoft.EntityFrameworkCore.Migrations;

namespace Cringe.Migrations.BeatmapDatabase
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Beatmaps",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BeatmapSetId = table.Column<int>("INTEGER", nullable: true),
                    Mode = table.Column<int>("INTEGER", nullable: false),
                    Md5 = table.Column<string>("TEXT", nullable: true),
                    Status = table.Column<int>("INTEGER", nullable: false),
                    Artist = table.Column<string>("TEXT", nullable: true),
                    Title = table.Column<string>("TEXT", nullable: true),
                    DifficultyName = table.Column<string>("TEXT", nullable: true),
                    Creator = table.Column<string>("TEXT", nullable: true),
                    Bpm = table.Column<double>("REAL", nullable: false),
                    HpDrain = table.Column<double>("REAL", nullable: false),
                    CircleSize = table.Column<double>("REAL", nullable: false),
                    OverallDifficulty = table.Column<double>("REAL", nullable: false),
                    ApproachRate = table.Column<double>("REAL", nullable: false),
                    Length = table.Column<int>("INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beatmaps", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Beatmaps");
        }
    }
}
