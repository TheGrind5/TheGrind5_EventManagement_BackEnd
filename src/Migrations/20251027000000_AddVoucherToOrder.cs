using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGrind5_EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VoucherCode",
                table: "Order",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Order",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoucherCode",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Order");
        }
    }
}

