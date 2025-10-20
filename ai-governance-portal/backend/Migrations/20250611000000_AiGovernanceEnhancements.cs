using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AiGovernanceEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdversarialFlag",
                table: "UsageEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AdversarialIndicators",
                table: "UsageEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AiConfidence",
                table: "UsageEntries",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiRationale",
                table: "UsageEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiRecommendation",
                table: "UsageEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiRiskLevel",
                table: "UsageEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "Unknown");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssessedAt",
                table: "UsageEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AutoDecisionSource",
                table: "UsageEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComplianceChecklist",
                table: "UsageEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "None");

            migrationBuilder.AddColumn<string>(
                name: "DenialReason",
                table: "UsageEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MajorViolations",
                table: "UsageEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModelName",
                table: "UsageEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModelVersion",
                table: "UsageEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PolicyAlerts",
                table: "UsageEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ModelCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsageEntryId = table.Column<int>(type: "INTEGER", nullable: false),
                    ApprovedBy = table.Column<string>(type: "TEXT", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FinalRiskLevel = table.Column<string>(type: "TEXT", nullable: false),
                    AiRiskLevel = table.Column<string>(type: "TEXT", nullable: false),
                    AiConfidence = table.Column<double>(type: "REAL", nullable: true),
                    AiRationale = table.Column<string>(type: "TEXT", nullable: true),
                    ComplianceChecklist = table.Column<string>(type: "TEXT", nullable: false),
                    StatusDecision = table.Column<string>(type: "TEXT", nullable: false),
                    PolicyAlerts = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelCards_UsageEntries_UsageEntryId",
                        column: x => x.UsageEntryId,
                        principalTable: "UsageEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelCards_UsageEntryId",
                table: "ModelCards",
                column: "UsageEntryId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelCards");

            migrationBuilder.DropColumn(
                name: "AdversarialFlag",
                table: "UsageEntries");

            migrationBuilder.DropColumn(
                name: "AdversarialIndicators",
                table: "UsageEntries");

            migrationBuilder.DropColumn(
                name: "AiConfidence",
                table: "UsageEntries");

            migrationBuilder.DropColumn(
                name: "AiRationale",
                table: "UsageEntries");

            migrationBuilder.DropColumn(
                name: "AiRecommendation",
                table: "UsageEntries");

            migrationBuilder.DropColumn(
                name: "AiRiskLevel",
                table: "UsageEntries");

            migrationBuilder.DropColumn(
                name: "AssessedAt",
                table: "UsageEntries");

            migrationBuilder.DropColumn(
                name: "AutoDecisionSource",
                table: "UsageEntries");

            migrationBuilder.DropColumn(
                name: "ComplianceChecklist",
                table: "UsageEntries");

            migrationBuilder.DropColumn(
                name: "DenialReason",
                table: "UsageEntries");

            migrationBuilder.DropColumn(
                name: "MajorViolations",
                table: "UsageEntries");

            migrationBuilder.DropColumn(
                name: "ModelName",
                table: "UsageEntries");

            migrationBuilder.DropColumn(
                name: "ModelVersion",
                table: "UsageEntries");

            migrationBuilder.DropColumn(
                name: "PolicyAlerts",
                table: "UsageEntries");
        }
    }
}
