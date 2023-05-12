using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyect_Rotten_Tomatoes.Migrations
{
    public partial class SecondCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Serie");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "Serie",
                newName: "Premiere_Date");

            migrationBuilder.AddColumn<bool>(
                name: "Top",
                table: "Serie",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Top",
                table: "Movie",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Top",
                table: "Serie");

            migrationBuilder.DropColumn(
                name: "Top",
                table: "Movie");

            migrationBuilder.RenameColumn(
                name: "Premiere_Date",
                table: "Serie",
                newName: "Rating");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Serie",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
