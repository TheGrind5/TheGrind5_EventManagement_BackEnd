using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGrind5_EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedEventCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Event",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BackgroundImage",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChildrenTerms",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventDetails",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventImage",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventIntroduction",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventMode",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialExperience",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialGuests",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                table: "Event",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TermsAndConditions",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VATTerms",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VenueName",
                table: "Event",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundImage",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "ChildrenTerms",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "EventDetails",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "EventImage",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "EventIntroduction",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "EventMode",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "SpecialExperience",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "SpecialGuests",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "StreetAddress",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "TermsAndConditions",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "VATTerms",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "VenueName",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "Event");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Event",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
