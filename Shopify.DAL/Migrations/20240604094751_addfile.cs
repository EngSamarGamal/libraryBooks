using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.DAL.Migrations
{
    public partial class addfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "file",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file",
                table: "Products");
        }
    }
}
