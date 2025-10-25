IF DB_ID('EventDB') IS NOT NULL
BEGIN
    ALTER DATABASE EventDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EventDB;
END;
GO
CREATE DATABASE EventDB COLLATE Vietnamese_CI_AI;
GO
USE EventDB;
GO

CREATE TABLE [User](
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    FullName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    Phone NVARCHAR(20),
    Role VARCHAR(16) NOT NULL CHECK (Role IN ('Customer','Host','Admin')),
    WalletBalance DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (WalletBalance >= 0),
    CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2(0),
    -- User Profile Fields (Added for profile management)
    Avatar NVARCHAR(MAX),           -- Profile avatar image path
    DateOfBirth DATETIME2,          -- User's date of birth
    Gender NVARCHAR(MAX)             -- User's gender
);

CREATE TABLE Event(
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    HostId INT NOT NULL,
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    StartTime DATETIME2(0) NOT NULL,
    EndTime DATETIME2(0) NOT NULL,
    Location NVARCHAR(255) NULL,
    EventType NVARCHAR(255) NULL,
    EventMode VARCHAR(10) NULL DEFAULT 'Offline' CHECK (EventMode IN ('Online','Offline')),
    Category NVARCHAR(255) NULL,
    Status VARCHAR(16) NULL DEFAULT 'Draft' CHECK (Status IN ('Draft','Open','Closed','Cancelled')),
    -- Gộp các thông tin bổ sung thành JSON để tiết kiệm không gian
    EventDetails NVARCHAR(MAX) NULL, -- JSON chứa: venue, images, introduction, special guests, etc.
    TermsAndConditions NVARCHAR(MAX) NULL, -- JSON chứa: terms, children terms, VAT terms
    OrganizerInfo NVARCHAR(MAX) NULL, -- JSON chứa: logo, name, info
    CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2(0) NULL,
    CONSTRAINT FK_Event_Host FOREIGN KEY (HostId) REFERENCES [User](UserId) ON DELETE CASCADE
);

CREATE TABLE TicketType(
    TicketTypeId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    TypeName NVARCHAR(100) NOT NULL,
    Price DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    Quantity INT NOT NULL CHECK (Quantity >= 0),
    MinOrder INT DEFAULT 1 CHECK (MinOrder >= 1),
    MaxOrder INT,
    SaleStart DATETIME2(0) NOT NULL,
    SaleEnd DATETIME2(0) NOT NULL,
    Status VARCHAR(16) NOT NULL DEFAULT 'Active' CHECK (Status IN ('Active','Inactive')),
    CONSTRAINT FK_TicketType_Event FOREIGN KEY (EventId) REFERENCES Event(EventId) ON DELETE CASCADE
);

CREATE TABLE [Order](
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL CHECK (Amount >= 0),
    Status VARCHAR(16) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending','Paid','Failed','Cancelled','Refunded')),
    PaymentMethod VARCHAR(50),
    CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2(0),
    CONSTRAINT FK_Order_Customer FOREIGN KEY (CustomerId) REFERENCES [User](UserId) ON DELETE CASCADE
);

CREATE TABLE OrderItem(
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    TicketTypeId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    SeatNo NVARCHAR(100),
    Status VARCHAR(16) DEFAULT 'Reserved' CHECK (Status IN ('Reserved','Confirmed','Cancelled')),
    CONSTRAINT FK_OrderItem_Order FOREIGN KEY (OrderId) REFERENCES [Order](OrderId) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItem_TicketType FOREIGN KEY (TicketTypeId) REFERENCES TicketType(TicketTypeId) ON DELETE NO ACTION
);

CREATE TABLE Ticket(
    TicketId INT IDENTITY(1,1) PRIMARY KEY,
    TicketTypeId INT NOT NULL,
    OrderItemId INT,
    SerialNumber NVARCHAR(100) NOT NULL UNIQUE,
    Status VARCHAR(16) NOT NULL DEFAULT 'Available' CHECK (Status IN ('Available','Assigned','Used','Refunded')),
    IssuedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UsedAt DATETIME2(0),
    RefundedAt DATETIME2(0),
    CONSTRAINT FK_Ticket_TicketType FOREIGN KEY (TicketTypeId) REFERENCES TicketType(TicketTypeId) ON DELETE NO ACTION,
    CONSTRAINT FK_Ticket_OrderItem FOREIGN KEY (OrderItemId) REFERENCES OrderItem(OrderItemId) ON DELETE SET NULL
);

CREATE TABLE Payment(
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL CHECK (Amount >= 0),
    Method VARCHAR(50) NOT NULL,
    Status VARCHAR(16) NOT NULL DEFAULT 'Initiated' CHECK (Status IN ('Initiated','Succeeded','Failed','Refunded')),
    PaymentDate DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Payment_Order FOREIGN KEY (OrderId) REFERENCES [Order](OrderId) ON DELETE CASCADE
);

-- Wishlist table (simplified - direct relationship with User and TicketType)
CREATE TABLE Wishlist(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    TicketTypeId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1 CHECK (Quantity >= 1),
    AddedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2(0),
    CONSTRAINT FK_Wishlist_User FOREIGN KEY (UserId) REFERENCES [User](UserId) ON DELETE CASCADE,
    CONSTRAINT FK_Wishlist_TicketType FOREIGN KEY (TicketTypeId) REFERENCES TicketType(TicketTypeId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Wishlist_User_TicketType UNIQUE (UserId, TicketTypeId)
);

-- Voucher table for discount management
CREATE TABLE Voucher(
    VoucherId INT IDENTITY(1,1) PRIMARY KEY,
    VoucherCode NVARCHAR(50) NOT NULL UNIQUE,
    DiscountPercentage DECIMAL(5,2) NOT NULL CHECK (DiscountPercentage >= 1 AND DiscountPercentage <= 100),
    ValidFrom DATETIME2(0) NOT NULL,
    ValidTo DATETIME2(0) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2(0)
);

-- Wallet functionality for users
-- WalletBalance: Stores user's wallet balance with precision (18,2)
-- Default value: 0 (new users start with 0 balance)
-- Constraint: Balance cannot be negative (CHECK WalletBalance >= 0)

-- Indexes (removed duplicate indexes - will be created later with correct naming)

-- ============================================
-- User Profile Fields Added:
-- ============================================
-- Avatar: Stores the file path to user's profile picture
-- DateOfBirth: User's birth date for age verification and personalization
-- Gender: User's gender for demographic analysis and personalization
-- ============================================


CREATE INDEX IX_Event_HostId ON Event(HostId);
CREATE INDEX IX_TicketType_EventId ON TicketType(EventId);
CREATE INDEX IX_Order_CustomerId ON [Order](CustomerId);
CREATE INDEX IX_OrderItem_OrderId ON OrderItem(OrderId);
CREATE INDEX IX_OrderItem_TicketTypeId ON OrderItem(TicketTypeId);
CREATE INDEX IX_Ticket_TicketTypeId ON Ticket(TicketTypeId);
CREATE INDEX IX_Ticket_OrderItemId ON Ticket(OrderItemId);
CREATE INDEX IX_Payment_OrderId ON Payment(OrderId);
CREATE INDEX IX_Wishlist_UserId ON Wishlist(UserId);
CREATE INDEX IX_Wishlist_TicketTypeId ON Wishlist(TicketTypeId);
CREATE INDEX IX_Wishlist_AddedAt ON Wishlist(AddedAt);
CREATE INDEX IX_Voucher_VoucherCode ON Voucher(VoucherCode);
CREATE INDEX IX_Voucher_ValidFrom ON Voucher(ValidFrom);
CREATE INDEX IX_Voucher_ValidTo ON Voucher(ValidTo);

-- WalletTransaction table for tracking wallet operations
CREATE TABLE WalletTransaction(
    TransactionId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL CHECK (Amount > 0),
    TransactionType VARCHAR(30) NOT NULL CHECK (TransactionType IN ('Deposit','Withdraw','Payment','Refund','Transfer_In','Transfer_Out')),
    Status VARCHAR(16) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending','Completed','Failed','Cancelled')),
    Description NVARCHAR(500),
    ReferenceId NVARCHAR(100), -- Reference to OrderId, PaymentId, etc.
    CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    CompletedAt DATETIME2(0),
    BalanceBefore DECIMAL(18,2) NOT NULL,
    BalanceAfter DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_WalletTransaction_User FOREIGN KEY (UserId) REFERENCES [User](UserId) ON DELETE CASCADE
);

-- OtpCode table for OTP verification
CREATE TABLE OtpCode(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL,
    Code NVARCHAR(10) NOT NULL,
    ExpiresAt DATETIME2(0) NOT NULL,
    IsUsed BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME()
);

-- Additional indexes for new tables
CREATE INDEX IX_WalletTransaction_UserId ON WalletTransaction(UserId);
CREATE INDEX IX_WalletTransaction_Status ON WalletTransaction(Status);
CREATE INDEX IX_WalletTransaction_CreatedAt ON WalletTransaction(CreatedAt);
CREATE INDEX IX_OtpCode_Email ON OtpCode(Email);
CREATE INDEX IX_OtpCode_ExpiresAt ON OtpCode(ExpiresAt);

-- Indexes for Event table (simplified)
CREATE INDEX IX_Event_EventMode ON Event(EventMode);
CREATE INDEX IX_Event_EventType ON Event(EventType);
CREATE INDEX IX_Event_Category ON Event(Category);
CREATE INDEX IX_Event_Status ON Event(Status);
CREATE INDEX IX_Event_StartTime ON Event(StartTime);
CREATE INDEX IX_Event_EndTime ON Event(EndTime);

-- Additional indexes for Voucher table
CREATE INDEX IX_Voucher_ValidFrom_ValidTo ON Voucher(ValidFrom, ValidTo);

