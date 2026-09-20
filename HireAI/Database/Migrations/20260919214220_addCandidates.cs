using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireAI.Database.Migrations;

/// <inheritdoc />
public partial class addCandidates : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Candidates",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Phone = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Candidates", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CandidatesExperiences",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                YearsOfExperience = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CandidatesExperiences", x => x.Id);
                table.ForeignKey(
                    name: "FK_CandidatesExperiences_Candidates_CandidateId",
                    column: x => x.CandidateId,
                    principalTable: "Candidates",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CandidatesSkills",
            columns: table => new
            {
                CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Skill = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CandidatesSkills", x => new { x.CandidateId, x.Skill });
                table.ForeignKey(
                    name: "FK_CandidatesSkills_Candidates_CandidateId",
                    column: x => x.CandidateId,
                    principalTable: "Candidates",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CandidatesExperiences_CandidateId",
            table: "CandidatesExperiences",
            column: "CandidateId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CandidatesExperiences");

        migrationBuilder.DropTable(
            name: "CandidatesSkills");

        migrationBuilder.DropTable(
            name: "Candidates");
    }
}
