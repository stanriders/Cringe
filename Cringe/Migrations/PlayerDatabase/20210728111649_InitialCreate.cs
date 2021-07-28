using Microsoft.EntityFrameworkCore.Migrations;

namespace Cringe.Migrations.PlayerDatabase
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    Accuracy = table.Column<float>(type: "REAL", nullable: false),
                    Playcount = table.Column<uint>(type: "INTEGER", nullable: false),
                    TotalScore = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Rank = table.Column<uint>(type: "INTEGER", nullable: false),
                    Pp = table.Column<ushort>(type: "INTEGER", nullable: false),
                    UserRank = table.Column<byte>(type: "INTEGER", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
