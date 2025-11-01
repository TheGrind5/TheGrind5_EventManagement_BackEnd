using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGrind5_EventManagement.Migrations
{
    /// <inheritdoc />
    public partial class FixOtpCodeAndVoucherPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if OtpCode table exists and has OtpId column, then rename to Id
            // If Id column already exists, skip rename
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                          WHERE TABLE_NAME = 'OtpCode' AND COLUMN_NAME = 'OtpId')
                AND NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'OtpCode' AND COLUMN_NAME = 'Id')
                BEGIN
                    EXEC sp_rename 'OtpCode.OtpId', 'Id', 'COLUMN';
                END
            ");

            // Alter DiscountPercentage precision only if column exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                          WHERE TABLE_NAME = 'Voucher' AND COLUMN_NAME = 'DiscountPercentage')
                BEGIN
                    ALTER TABLE [Voucher] 
                    ALTER COLUMN [DiscountPercentage] DECIMAL(5,2) NOT NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert: Rename Id back to OtpId if Id exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                          WHERE TABLE_NAME = 'OtpCode' AND COLUMN_NAME = 'Id')
                AND NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'OtpCode' AND COLUMN_NAME = 'OtpId')
                BEGIN
                    EXEC sp_rename 'OtpCode.Id', 'OtpId', 'COLUMN';
                END
            ");

            // Revert DiscountPercentage precision
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                          WHERE TABLE_NAME = 'Voucher' AND COLUMN_NAME = 'DiscountPercentage')
                BEGIN
                    ALTER TABLE [Voucher] 
                    ALTER COLUMN [DiscountPercentage] DECIMAL(18,2) NOT NULL;
                END
            ");
        }
    }
}
