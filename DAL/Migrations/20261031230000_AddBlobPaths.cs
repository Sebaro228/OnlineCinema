using DAL.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(CinemaContext))]
    [Migration("20261031230000_AddBlobPaths")]
    public partial class AddBlobPaths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "PosterBlobPath", table: "Films", type: "nvarchar(max)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "VideoBlobPath", table: "Films", type: "nvarchar(max)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "BlobPath", table: "Trailers", type: "nvarchar(max)", nullable: false, defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PosterBlobPath", table: "Films");
            migrationBuilder.DropColumn(name: "VideoBlobPath", table: "Films");
            migrationBuilder.DropColumn(name: "BlobPath", table: "Trailers");
        }
    }
}
