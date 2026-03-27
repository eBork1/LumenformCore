using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumenform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByUserIdToCohort : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Cohorts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Cohorts_CreatedByUserId",
                table: "Cohorts",
                column: "CreatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cohorts_CreatedByUserId",
                table: "Cohorts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Cohorts");
        }
    }
}
