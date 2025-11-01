using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGrind5_EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddEventIdToOrderAndEventQuestionSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kiểm tra và thêm EventId nếu chưa tồn tại (cột có thể đã có trong TheGrind5_Query.sql)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Order' AND COLUMN_NAME = 'EventId'
                )
                BEGIN
                    ALTER TABLE [Order] ADD [EventId] int NOT NULL DEFAULT 0;
                END
            ");

            // Kiểm tra và thêm OrderAnswers nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Order' AND COLUMN_NAME = 'OrderAnswers'
                )
                BEGIN
                    ALTER TABLE [Order] ADD [OrderAnswers] nvarchar(max) NULL;
                END
            ");

            // Kiểm tra và tạo bảng EventQuestion nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EventQuestion')
                BEGIN
                    CREATE TABLE [EventQuestion] (
                        [QuestionId] int NOT NULL IDENTITY(1,1),
                        [EventId] int NOT NULL,
                        [QuestionText] nvarchar(500) NOT NULL,
                        [QuestionType] nvarchar(max) NOT NULL,
                        [IsRequired] bit NOT NULL,
                        [Options] nvarchar(max) NULL,
                        [Placeholder] nvarchar(max) NULL,
                        [ValidationRules] nvarchar(max) NULL,
                        [DisplayOrder] int NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NULL,
                        CONSTRAINT [PK_EventQuestion] PRIMARY KEY ([QuestionId])
                    );
                END
            ");

            // Kiểm tra và tạo Index cho Order.EventId nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM sys.indexes 
                    WHERE name = 'IX_Order_EventId' 
                    AND object_id = OBJECT_ID('Order')
                )
                BEGIN
                    CREATE INDEX [IX_Order_EventId] ON [Order] ([EventId]);
                END
            ");

            // Kiểm tra và tạo Index cho EventQuestion.EventId nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM sys.indexes 
                    WHERE name = 'IX_EventQuestion_EventId' 
                    AND object_id = OBJECT_ID('EventQuestion')
                )
                BEGIN
                    CREATE INDEX [IX_EventQuestion_EventId] ON [EventQuestion] ([EventId]);
                END
            ");

            // Kiểm tra và tạo Foreign Key cho EventQuestion nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE TABLE_NAME = 'EventQuestion' 
                    AND CONSTRAINT_NAME = 'FK_EventQuestion_Event_EventId'
                )
                BEGIN
                    ALTER TABLE [EventQuestion]
                    ADD CONSTRAINT [FK_EventQuestion_Event_EventId] 
                    FOREIGN KEY ([EventId]) REFERENCES [Event] ([EventId]) ON DELETE CASCADE;
                END
            ");

            // Kiểm tra và tạo Foreign Key cho Order.EventId nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE TABLE_NAME = 'Order' 
                    AND CONSTRAINT_NAME = 'FK_Order_Event_EventId'
                )
                BEGIN
                    ALTER TABLE [Order]
                    ADD CONSTRAINT [FK_Order_Event_EventId] 
                    FOREIGN KEY ([EventId]) REFERENCES [Event] ([EventId]) ON DELETE NO ACTION;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Event_EventId",
                table: "Order");

            migrationBuilder.DropTable(
                name: "EventQuestion");

            migrationBuilder.DropIndex(
                name: "IX_Order_EventId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderAnswers",
                table: "Order");
        }
    }
}
