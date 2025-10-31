using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGrind5_EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCampusModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "Event",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Campus",
                columns: table => new
                {
                    CampusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campus", x => x.CampusId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_CampusId",
                table: "User",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CampusId",
                table: "Event",
                column: "CampusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Campus_CampusId",
                table: "Event",
                column: "CampusId",
                principalTable: "Campus",
                principalColumn: "CampusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Campus_CampusId",
                table: "User",
                column: "CampusId",
                principalTable: "Campus",
                principalColumn: "CampusId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Campus_CampusId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Campus_CampusId",
                table: "User");

            migrationBuilder.DropTable(
                name: "Campus");

            migrationBuilder.DropIndex(
                name: "IX_User_CampusId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Event_CampusId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Event");
        }
    }
}
