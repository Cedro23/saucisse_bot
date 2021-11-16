using Microsoft.EntityFrameworkCore.Migrations;

namespace Saucisse_bot.DAL.Migrations.Migrations
{
    public partial class AddedGuildIdToItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GuildId",
                table: "Items",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "Items");
        }
    }
}
