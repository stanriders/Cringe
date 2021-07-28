using Microsoft.EntityFrameworkCore.Migrations;

namespace Cringe.Migrations.BeatmapDatabase
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beatmaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BeatmapSetId = table.Column<int>(type: "INTEGER", nullable: true),
                    Mode = table.Column<int>(type: "INTEGER", nullable: false),
                    Md5 = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Artist = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    DifficultyName = table.Column<string>(type: "TEXT", nullable: true),
                    Creator = table.Column<string>(type: "TEXT", nullable: true),
                    Bpm = table.Column<double>(type: "REAL", nullable: false),
                    HpDrain = table.Column<double>(type: "REAL", nullable: false),
                    CircleSize = table.Column<double>(type: "REAL", nullable: false),
                    OverallDifficulty = table.Column<double>(type: "REAL", nullable: false),
                    ApproachRate = table.Column<double>(type: "REAL", nullable: false),
                    Length = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beatmaps", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beatmaps");
        }
    }
}
