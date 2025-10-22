using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGrind5_EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizerInfo",
                table: "Event",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizerLogo",
                table: "Event",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizerName",
                table: "Event",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizerInfo",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "OrganizerLogo",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "OrganizerName",
                table: "Event");
        }
    }
}
