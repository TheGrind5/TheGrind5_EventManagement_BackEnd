using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGrind5_EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCampusModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if CampusId column exists in User table before adding
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('User') AND name = 'CampusId')
                BEGIN
                    ALTER TABLE [User] ADD [CampusId] int NULL;
                END
            ");

            // Check if CampusId column exists in Event table before adding
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Event') AND name = 'CampusId')
                BEGIN
                    ALTER TABLE [Event] ADD [CampusId] int NULL;
                END
            ");

            // Check if Campus table exists before creating
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Campus')
                BEGIN
                    CREATE TABLE [Campus] (
                        [CampusId] int NOT NULL IDENTITY(1,1),
                        [Name] nvarchar(100) NOT NULL,
                        [Code] nvarchar(50) NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NULL,
                        CONSTRAINT [PK_Campus] PRIMARY KEY ([CampusId])
                    );
                END
            ");

            // Check if index exists before creating
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_User_CampusId' AND object_id = OBJECT_ID('User'))
                BEGIN
                    CREATE INDEX [IX_User_CampusId] ON [User] ([CampusId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Event_CampusId' AND object_id = OBJECT_ID('Event'))
                BEGIN
                    CREATE INDEX [IX_Event_CampusId] ON [Event] ([CampusId]);
                END
            ");

            // Check if foreign key exists before creating
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Event_Campus_CampusId')
                BEGIN
                    ALTER TABLE [Event] ADD CONSTRAINT [FK_Event_Campus_CampusId] 
                    FOREIGN KEY ([CampusId]) REFERENCES [Campus] ([CampusId]) ON DELETE NO ACTION;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_User_Campus_CampusId')
                BEGIN
                    ALTER TABLE [User] ADD CONSTRAINT [FK_User_Campus_CampusId] 
                    FOREIGN KEY ([CampusId]) REFERENCES [Campus] ([CampusId]) ON DELETE NO ACTION;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Safely drop foreign keys if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Event_Campus_CampusId')
                BEGIN
                    ALTER TABLE [Event] DROP CONSTRAINT [FK_Event_Campus_CampusId];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_User_Campus_CampusId')
                BEGIN
                    ALTER TABLE [User] DROP CONSTRAINT [FK_User_Campus_CampusId];
                END
            ");

            // Drop table if exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Campus')
                BEGIN
                    DROP TABLE [Campus];
                END
            ");

            // Drop indexes if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_User_CampusId' AND object_id = OBJECT_ID('User'))
                BEGIN
                    DROP INDEX [IX_User_CampusId] ON [User];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Event_CampusId' AND object_id = OBJECT_ID('Event'))
                BEGIN
                    DROP INDEX [IX_Event_CampusId] ON [Event];
                END
            ");

            // Drop columns if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('User') AND name = 'CampusId')
                BEGIN
                    ALTER TABLE [User] DROP COLUMN [CampusId];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Event') AND name = 'CampusId')
                BEGIN
                    ALTER TABLE [Event] DROP COLUMN [CampusId];
                END
            ");
        }
    }
}
