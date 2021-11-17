using Microsoft.EntityFrameworkCore.Migrations;

namespace Saucisse_bot.DAL.Migrations.Migrations
{
    public partial class UpdatedAvatarUrlToImageUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                table: "Items",
                newName: "ImageUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Items",
                newName: "AvatarUrl");
        }
    }
}
