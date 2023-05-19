using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyect_Rotten_Tomatoes.Migrations
{
    public partial class favouritesCinephile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavouriteMovies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    CinephileId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteMovies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavouriteMovies_Cinephile_CinephileId",
                        column: x => x.CinephileId,
                        principalTable: "Cinephile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavouriteMovies_Movie_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavouriteSeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SerieId = table.Column<int>(type: "int", nullable: false),
                    CinephileId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavouriteSeries_Cinephile_CinephileId",
                        column: x => x.CinephileId,
                        principalTable: "Cinephile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavouriteSeries_Serie_SerieId",
                        column: x => x.SerieId,
                        principalTable: "Serie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteMovies_CinephileId",
                table: "FavouriteMovies",
                column: "CinephileId");

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteMovies_MovieId",
                table: "FavouriteMovies",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteSeries_CinephileId",
                table: "FavouriteSeries",
                column: "CinephileId");

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteSeries_SerieId",
                table: "FavouriteSeries",
                column: "SerieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavouriteMovies");

            migrationBuilder.DropTable(
                name: "FavouriteSeries");
        }
    }
}
