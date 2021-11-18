using Microsoft.EntityFrameworkCore.Migrations;

namespace Saucisse_bot.DAL.Migrations.Migrations
{
    public partial class AddedMaxBuyableQuantityToItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxBuyableQuantiy",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxBuyableQuantiy",
                table: "Items");
        }
    }
}
