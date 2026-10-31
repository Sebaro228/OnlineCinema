using DAL.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(CinemaContext))]
    [Migration("20261031231000_BackfillLegacyBlobPaths")]
    public partial class BackfillLegacyBlobPaths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Populate path fields for files created before AddBlobPaths.
            migrationBuilder.Sql(@"UPDATE [Films]
                SET [PosterBlobPath] = CONCAT('posters/', [Id], [PosterExtension])
                WHERE ([PosterBlobPath] IS NULL OR [PosterBlobPath] = '')
                  AND [PosterExtension] IS NOT NULL AND [PosterExtension] <> '';");

            migrationBuilder.Sql(@"UPDATE [Films]
                SET [VideoBlobPath] = CONCAT('videos/', [Id], [VideoExtension])
                WHERE ([VideoBlobPath] IS NULL OR [VideoBlobPath] = '')
                  AND [VideoExtension] IS NOT NULL AND [VideoExtension] <> '';");

            migrationBuilder.Sql(@"UPDATE [Trailers]
                SET [BlobPath] = CONCAT('trailers/', [FilmId], '/', [Id], [Extension])
                WHERE ([BlobPath] IS NULL OR [BlobPath] = '')
                  AND [Extension] IS NOT NULL AND [Extension] <> '';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Deliberately do not erase paths: existing records may still need them.
        }
    }
}
