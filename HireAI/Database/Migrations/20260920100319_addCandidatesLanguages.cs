using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireAI.Database.Migrations;

/// <inheritdoc />
public partial class addCandidatesLanguages : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CandidatesLanguages",
            columns: table => new
            {
                CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Language = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Level = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CandidatesLanguages", x => new { x.CandidateId, x.Language });
                table.ForeignKey(
                    name: "FK_CandidatesLanguages_Candidates_CandidateId",
                    column: x => x.CandidateId,
                    principalTable: "Candidates",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CandidatesLanguages");
    }
}
