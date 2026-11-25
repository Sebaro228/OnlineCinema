using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class Favorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilmUser",
                columns: table => new
                {
                    FavoritedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FavoritesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmUser", x => new { x.FavoritedById, x.FavoritesId });
                    table.ForeignKey(
                        name: "FK_FilmUser_AspNetUsers_FavoritedById",
                        column: x => x.FavoritedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmUser_Films_FavoritesId",
                        column: x => x.FavoritesId,
                        principalTable: "Films",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmUser_FavoritesId",
                table: "FilmUser",
                column: "FavoritesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmUser");
        }
    }
}
