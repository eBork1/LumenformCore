using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumenform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cohorts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ParishName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cohorts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CohortId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsTemplate = table.Column<bool>(type: "boolean", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Cohorts_CohortId",
                        column: x => x.CohortId,
                        principalTable: "Cohorts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CohortMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CohortId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ParticipantType = table.Column<int>(type: "integer", nullable: true),
                    SponsorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    JoinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CohortMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CohortMemberships_Cohorts_CohortId",
                        column: x => x.CohortId,
                        principalTable: "Cohorts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentSubmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CoordinatorFeedback = table.Column<string>(type: "text", nullable: true),
                    FeedbackProvidedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FeedbackProvidedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentSubmissions_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentTasks_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentTaskCompletions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentTaskCompletions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentTaskCompletions_AssignmentTasks_AssignmentTaskId",
                        column: x => x.AssignmentTaskId,
                        principalTable: "AssignmentTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CohortId",
                table: "Assignments",
                column: "CohortId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CreatedByUserId",
                table: "Assignments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_DueDate",
                table: "Assignments",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_IsTemplate",
                table: "Assignments",
                column: "IsTemplate");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_AssignmentId",
                table: "AssignmentSubmissions",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_AssignmentId_UserId",
                table: "AssignmentSubmissions",
                columns: new[] { "AssignmentId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_UserId",
                table: "AssignmentSubmissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentTaskCompletions_AssignmentTaskId",
                table: "AssignmentTaskCompletions",
                column: "AssignmentTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentTaskCompletions_AssignmentTaskId_UserId",
                table: "AssignmentTaskCompletions",
                columns: new[] { "AssignmentTaskId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentTaskCompletions_UserId",
                table: "AssignmentTaskCompletions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentTasks_AssignmentId",
                table: "AssignmentTasks",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentTasks_AssignmentId_Order",
                table: "AssignmentTasks",
                columns: new[] { "AssignmentId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_CohortMemberships_CohortId_UserId",
                table: "CohortMemberships",
                columns: new[] { "CohortId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CohortMemberships_Status",
                table: "CohortMemberships",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CohortMemberships_UserId",
                table: "CohortMemberships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cohorts_CreatedAt",
                table: "Cohorts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Cohorts_IsActive",
                table: "Cohorts",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentSubmissions");

            migrationBuilder.DropTable(
                name: "AssignmentTaskCompletions");

            migrationBuilder.DropTable(
                name: "CohortMemberships");

            migrationBuilder.DropTable(
                name: "AssignmentTasks");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Cohorts");
        }
    }
}
