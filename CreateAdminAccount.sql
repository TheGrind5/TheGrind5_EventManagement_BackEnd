-- ========================================
-- TheGrind5 Event Management
-- Create Admin Account Script
-- ========================================

USE EventDB;
GO

-- ========================================
-- CREATE ADMIN ACCOUNT
-- ========================================
-- 
-- PASSWORD: 123456 (same hash as other users)
-- Hash được tạo bằng bcrypt: $2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u
-- 
-- LOGIN CREDENTIALS:
-- Email: admin@thegrind5.com
-- Password: 123456
-- Role: Admin
-- ========================================

-- Kiểm tra xem admin đã tồn tại chưa
IF NOT EXISTS (SELECT 1 FROM [User] WHERE Email = 'admin@thegrind5.com')
BEGIN
    INSERT INTO [User] (
        Username, 
        FullName, 
        Email, 
        PasswordHash, 
        Phone, 
        Role, 
        WalletBalance, 
        CreatedAt, 
        UpdatedAt, 
        Avatar, 
        DateOfBirth, 
        Gender
    )
    VALUES (
        'admin',                                                                    -- Username
        N'Quản Trị Viên',                                                          -- FullName
        'admin@thegrind5.com',                                                      -- Email
        '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',           -- PasswordHash (123456)
        '0999999999',                                                               -- Phone
        'Admin',                                                                    -- Role
        0.00,                                                                       -- WalletBalance
        GETUTCDATE(),                                                               -- CreatedAt
        GETUTCDATE(),                                                               -- UpdatedAt
        NULL,                                                                       -- Avatar
        '1990-01-01',                                                               -- DateOfBirth
        N'Nam'                                                                      -- Gender
    );
    
    PRINT '✅ Admin account created successfully!';
    PRINT '';
    PRINT '========================================';
    PRINT 'ADMIN ACCOUNT INFORMATION';
    PRINT '========================================';
    PRINT 'Email: admin@thegrind5.com';
    PRINT 'Password: 123456';
    PRINT 'Role: Admin';
    PRINT '========================================';
END
ELSE
BEGIN
    PRINT '⚠️  Admin account already exists!';
    PRINT 'Email: admin@thegrind5.com';
END

-- Hiển thị thông tin Admin account
SELECT 
    UserId,
    Username,
    FullName,
    Email,
    Role,
    WalletBalance,
    CreatedAt
FROM [User]
WHERE Email = 'admin@thegrind5.com';

GO

