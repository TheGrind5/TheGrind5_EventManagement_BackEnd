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

            // Xóa CHECK constraint trước khi thay đổi cột Status (nếu constraint tồn tại)
            migrationBuilder.Sql(@"
                DECLARE @constraintName sysname;
                SELECT @constraintName = name
                FROM sys.check_constraints
                WHERE parent_object_id = OBJECT_ID(N'[Order]')
                AND name LIKE 'CK__Order__Status%';
                
                IF @constraintName IS NOT NULL
                BEGIN
                    EXEC('ALTER TABLE [Order] DROP CONSTRAINT [' + @constraintName + ']');
                END
            ");

            // Xóa default constraint nếu có
            migrationBuilder.Sql(@"
                DECLARE @defaultName sysname;
                SELECT @defaultName = d.name
                FROM sys.default_constraints d
                INNER JOIN sys.columns c ON d.parent_column_id = c.column_id AND d.parent_object_id = c.object_id
                WHERE d.parent_object_id = OBJECT_ID(N'[Order]') AND c.name = N'Status';
                
                IF @defaultName IS NOT NULL
                BEGIN
                    EXEC('ALTER TABLE [Order] DROP CONSTRAINT [' + @defaultName + ']');
                END
            ");

            // Cập nhật giá trị NULL thành empty string
            migrationBuilder.Sql("UPDATE [Order] SET [Status] = N'' WHERE [Status] IS NULL");

            // Thay đổi cột Status
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // Thay đổi cột PaymentMethod
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
