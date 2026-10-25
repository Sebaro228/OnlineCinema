using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class V5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trailer_Films_FilmId",
                table: "Trailer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trailer",
                table: "Trailer");

            migrationBuilder.RenameTable(
                name: "Trailer",
                newName: "Trailers");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Trailers",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_Trailer_FilmId",
                table: "Trailers",
                newName: "IX_Trailers_FilmId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trailers",
                table: "Trailers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trailers_Films_FilmId",
                table: "Trailers",
                column: "FilmId",
                principalTable: "Films",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trailers_Films_FilmId",
                table: "Trailers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trailers",
                table: "Trailers");

            migrationBuilder.RenameTable(
                name: "Trailers",
                newName: "Trailer");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Trailer",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Trailers_FilmId",
                table: "Trailer",
                newName: "IX_Trailer_FilmId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trailer",
                table: "Trailer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trailer_Films_FilmId",
                table: "Trailer",
                column: "FilmId",
                principalTable: "Films",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
