using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGrind5_EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddUserBanFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kiểm tra và thêm BanReason nếu chưa tồn tại (cột có thể đã có trong TheGrind5_Query.sql)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'User' AND COLUMN_NAME = 'BanReason'
                )
                BEGIN
                    ALTER TABLE [User] ADD [BanReason] nvarchar(max) NULL;
                END
            ");

            // Kiểm tra và thêm BannedAt nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'User' AND COLUMN_NAME = 'BannedAt'
                )
                BEGIN
                    ALTER TABLE [User] ADD [BannedAt] datetime2 NULL;
                END
            ");

            // Kiểm tra và thêm IsBanned nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'User' AND COLUMN_NAME = 'IsBanned'
                )
                BEGIN
                    ALTER TABLE [User] ADD [IsBanned] bit NOT NULL DEFAULT 0;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BanReason",
                table: "User");

            migrationBuilder.DropColumn(
                name: "BannedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "User");
        }
    }
}
