using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class FilmParticipationMultipleRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParticipantRole",
                table: "FilmParticipation",
                newName: "ParticipantRoles");

            migrationBuilder.RenameIndex(
                name: "IX_FilmParticipation_FilmId_ParticipantId_ParticipantRole",
                table: "FilmParticipation",
                newName: "IX_FilmParticipation_FilmId_ParticipantId_ParticipantRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParticipantRoles",
                table: "FilmParticipation",
                newName: "ParticipantRole");

            migrationBuilder.RenameIndex(
                name: "IX_FilmParticipation_FilmId_ParticipantId_ParticipantRoles",
                table: "FilmParticipation",
                newName: "IX_FilmParticipation_FilmId_ParticipantId_ParticipantRole");
        }
    }
}
