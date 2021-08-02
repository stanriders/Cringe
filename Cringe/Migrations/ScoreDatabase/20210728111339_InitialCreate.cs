using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cringe.Migrations.ScoreDatabase
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Scores",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Count300 = table.Column<int>("INTEGER", nullable: false),
                    Count100 = table.Column<int>("INTEGER", nullable: false),
                    Count50 = table.Column<int>("INTEGER", nullable: false),
                    CountGeki = table.Column<int>("INTEGER", nullable: false),
                    CountKatu = table.Column<int>("INTEGER", nullable: false),
                    CountMiss = table.Column<int>("INTEGER", nullable: false),
                    Score = table.Column<long>("INTEGER", nullable: false),
                    MaxCombo = table.Column<int>("INTEGER", nullable: false),
                    FullCombo = table.Column<bool>("INTEGER", nullable: false),
                    Rank = table.Column<string>("TEXT", nullable: true),
                    Mods = table.Column<int>("INTEGER", nullable: false),
                    Passed = table.Column<bool>("INTEGER", nullable: false),
                    GameMode = table.Column<int>("INTEGER", nullable: false),
                    PlayDateTime = table.Column<DateTime>("TEXT", nullable: false),
                    OsuVersion = table.Column<string>("TEXT", nullable: true),
                    Quit = table.Column<bool>("INTEGER", nullable: false),
                    Failed = table.Column<bool>("INTEGER", nullable: false),
                    Pp = table.Column<double>("REAL", nullable: false),
                    BeatmapId = table.Column<int>("INTEGER", nullable: false),
                    PlayerId = table.Column<int>("INTEGER", nullable: false),
                    PlayerUsername = table.Column<string>("TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Scores");
        }
    }
}
