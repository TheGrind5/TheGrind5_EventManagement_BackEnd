using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGrind5_EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kiểm tra bảng Notification đã tồn tại chưa (có thể đã được tạo từ TheGrind5_Query.sql)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Notification')
                BEGIN
                    CREATE TABLE [Notification] (
                        [NotificationId] int NOT NULL IDENTITY(1,1),
                        [UserId] int NOT NULL,
                        [Title] nvarchar(200) NOT NULL,
                        [Content] nvarchar(max) NULL,
                        [Type] nvarchar(max) NOT NULL,
                        [IsRead] bit NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [ReadAt] datetime2 NULL,
                        [RelatedEventId] int NULL,
                        [RelatedOrderId] int NULL,
                        [RelatedTicketId] int NULL,
                        CONSTRAINT [PK_Notification] PRIMARY KEY ([NotificationId])
                    );
                END
            ");

            // Kiểm tra và tạo Foreign Key nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE TABLE_NAME = 'Notification' 
                    AND CONSTRAINT_TYPE = 'FOREIGN KEY'
                )
                BEGIN
                    ALTER TABLE [Notification]
                    ADD CONSTRAINT [FK_Notification_User] 
                    FOREIGN KEY ([UserId]) REFERENCES [User] ([UserID]) ON DELETE CASCADE;
                END
            ");

            // Kiểm tra và tạo Index nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM sys.indexes 
                    WHERE name = 'IX_Notification_UserId' 
                    AND object_id = OBJECT_ID('Notification')
                )
                BEGIN
                    CREATE INDEX [IX_Notification_UserId] ON [Notification] ([UserId]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification");
        }
    }
}
