using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyect_Rotten_Tomatoes.Migrations
{
    public partial class SecondCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Release_Date",
                table: "Movie",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Release_Date",
                table: "Movie");
        }
    }
}
