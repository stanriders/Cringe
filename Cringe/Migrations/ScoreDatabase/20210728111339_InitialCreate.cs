using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cringe.Migrations.ScoreDatabase
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Count300 = table.Column<int>(type: "INTEGER", nullable: false),
                    Count100 = table.Column<int>(type: "INTEGER", nullable: false),
                    Count50 = table.Column<int>(type: "INTEGER", nullable: false),
                    CountGeki = table.Column<int>(type: "INTEGER", nullable: false),
                    CountKatu = table.Column<int>(type: "INTEGER", nullable: false),
                    CountMiss = table.Column<int>(type: "INTEGER", nullable: false),
                    Score = table.Column<long>(type: "INTEGER", nullable: false),
                    MaxCombo = table.Column<int>(type: "INTEGER", nullable: false),
                    FullCombo = table.Column<bool>(type: "INTEGER", nullable: false),
                    Rank = table.Column<string>(type: "TEXT", nullable: true),
                    Mods = table.Column<int>(type: "INTEGER", nullable: false),
                    Passed = table.Column<bool>(type: "INTEGER", nullable: false),
                    GameMode = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OsuVersion = table.Column<string>(type: "TEXT", nullable: true),
                    Quit = table.Column<bool>(type: "INTEGER", nullable: false),
                    Failed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Pp = table.Column<double>(type: "REAL", nullable: false),
                    BeatmapId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerUsername = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Scores");
        }
    }
}
