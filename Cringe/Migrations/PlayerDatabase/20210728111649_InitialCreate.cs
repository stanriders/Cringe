using Microsoft.EntityFrameworkCore.Migrations;

namespace Cringe.Migrations.PlayerDatabase
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Players",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>("TEXT", nullable: true),
                    Accuracy = table.Column<float>("REAL", nullable: false),
                    Playcount = table.Column<uint>("INTEGER", nullable: false),
                    TotalScore = table.Column<ulong>("INTEGER", nullable: false),
                    Rank = table.Column<uint>("INTEGER", nullable: false),
                    Pp = table.Column<ushort>("INTEGER", nullable: false),
                    UserRank = table.Column<byte>("INTEGER", nullable: false),
                    Password = table.Column<string>("TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Players");
        }
    }
}
