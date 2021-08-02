using Microsoft.EntityFrameworkCore.Migrations;

namespace Cringe.Migrations.PlayerDatabase
{
    public partial class AddFriendsKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                    "Id",
                    "Friends",
                    "INTEGER",
                    nullable: false,
                    defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                "PK_Friends",
                "Friends",
                "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                "PK_Friends",
                "Friends");

            migrationBuilder.DropColumn(
                "Id",
                "Friends");
        }
    }
}
