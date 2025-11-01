using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGrind5_EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class SyncAfterFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No-op for user ban columns; already added in previous migration

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op for user ban columns

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
