using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGrind5_EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddVNPayFieldsToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kiểm tra và thêm các cột VNPay nếu chưa tồn tại (có thể đã có trong TheGrind5_Query.sql)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Payment' AND COLUMN_NAME = 'CreatedAt'
                )
                BEGIN
                    ALTER TABLE [Payment] ADD [CreatedAt] datetime2 NOT NULL DEFAULT SYSDATETIME();
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Payment' AND COLUMN_NAME = 'UpdatedAt'
                )
                BEGIN
                    ALTER TABLE [Payment] ADD [UpdatedAt] datetime2 NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Payment' AND COLUMN_NAME = 'TransactionId'
                )
                BEGIN
                    ALTER TABLE [Payment] ADD [TransactionId] nvarchar(max) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Payment' AND COLUMN_NAME = 'VnpTxnRef'
                )
                BEGIN
                    ALTER TABLE [Payment] ADD [VnpTxnRef] nvarchar(max) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Payment' AND COLUMN_NAME = 'ResponseCode'
                )
                BEGIN
                    ALTER TABLE [Payment] ADD [ResponseCode] nvarchar(max) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Payment' AND COLUMN_NAME = 'TransactionStatus'
                )
                BEGIN
                    ALTER TABLE [Payment] ADD [TransactionStatus] nvarchar(max) NULL;
                END
            ");

            // Không tạo index cho TransactionId và VnpTxnRef vì chúng là nvarchar(max)
            // SQL Server không cho phép tạo index cho nvarchar(max)
            // Index sẽ được tạo bởi SQL script nếu cần (với cột có kích thước cụ thể)
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "ResponseCode",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "TransactionStatus",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "VnpTxnRef",
                table: "Payment");
        }
    }
}
