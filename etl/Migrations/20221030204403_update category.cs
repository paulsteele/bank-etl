using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace core.Migrations
{
    public partial class updatecategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FireflyOrder",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FireflyOrder",
                table: "Categories");
        }
    }
}
