using Microsoft.EntityFrameworkCore.Migrations;

namespace Saucisse_bot.DAL.Migrations.Migrations
{
    public partial class AddedQuantityToProfileItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ProfileItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProfileItems");
        }
    }
}
