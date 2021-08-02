using Microsoft.EntityFrameworkCore.Migrations;

namespace Cringe.Migrations.PlayerDatabase
{
    public partial class AddFriends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Friends",
                table => new
                {
                    FromId = table.Column<int>("INTEGER", nullable: true),
                    ToId = table.Column<int>("INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        "FK_Friends_Players_FromId",
                        x => x.FromId,
                        "Players",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Friends_Players_ToId",
                        x => x.ToId,
                        "Players",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Friends_FromId",
                "Friends",
                "FromId");

            migrationBuilder.CreateIndex(
                "IX_Friends_ToId",
                "Friends",
                "ToId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Friends");
        }
    }
}
