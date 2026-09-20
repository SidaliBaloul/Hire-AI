using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireAI.Database.Migrations;

/// <inheritdoc />
public partial class addCandidateUserRelationship : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "userId",
            table: "Candidates",
            type: "uniqueidentifier",
            nullable: true,
            defaultValue: Guid.Empty);

        migrationBuilder.CreateIndex(
            name: "IX_Candidates_userId",
            table: "Candidates",
            column: "userId");

        migrationBuilder.AddForeignKey(
            name: "FK_Candidates_Users_userId",
            table: "Candidates",
            column: "userId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Candidates_Users_userId",
            table: "Candidates");

        migrationBuilder.DropIndex(
            name: "IX_Candidates_userId",
            table: "Candidates");

        migrationBuilder.DropColumn(
            name: "userId",
            table: "Candidates");
    }
}
