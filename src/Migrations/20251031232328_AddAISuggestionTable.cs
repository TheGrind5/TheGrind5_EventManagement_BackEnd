using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGrind5_EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddAISuggestionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa bảng EventReport nếu tồn tại
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EventReport')
                BEGIN
                    DROP TABLE [EventReport];
                END
            ");

            // Kiểm tra và tạo bảng AISuggestion nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AISuggestion')
                BEGIN
                    CREATE TABLE [AISuggestion] (
                        [SuggestionId] int NOT NULL IDENTITY(1,1),
                        [UserId] int NOT NULL,
                        [SuggestionType] nvarchar(max) NOT NULL,
                        [RequestData] nvarchar(max) NULL,
                        [ResponseData] nvarchar(max) NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_AISuggestion] PRIMARY KEY ([SuggestionId])
                    );
                END
            ");

            // Kiểm tra và tạo Foreign Key nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE TABLE_NAME = 'AISuggestion' 
                    AND CONSTRAINT_NAME = 'FK_AISuggestion_User_UserId'
                )
                BEGIN
                    ALTER TABLE [AISuggestion]
                    ADD CONSTRAINT [FK_AISuggestion_User_UserId] 
                    FOREIGN KEY ([UserId]) REFERENCES [User] ([UserID]) ON DELETE NO ACTION;
                END
            ");

            // Kiểm tra và tạo Index nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM sys.indexes 
                    WHERE name = 'IX_AISuggestion_UserId' 
                    AND object_id = OBJECT_ID('AISuggestion')
                )
                BEGIN
                    CREATE INDEX [IX_AISuggestion_UserId] ON [AISuggestion] ([UserId]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AISuggestion");

            migrationBuilder.CreateTable(
                name: "EventReport",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ReportReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventReport", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_EventReport_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventReport_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventReport_EventId",
                table: "EventReport",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventReport_UserId_EventId",
                table: "EventReport",
                columns: new[] { "UserId", "EventId" },
                unique: true);
        }
    }
}
