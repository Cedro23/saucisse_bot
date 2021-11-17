using Microsoft.EntityFrameworkCore.Migrations;

namespace Saucisse_bot.DAL.Migrations.Migrations
{
    public partial class AddedRarityToItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rarity",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rarity",
                table: "Items");
        }
    }
}
