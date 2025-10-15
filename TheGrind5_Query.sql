IF DB_ID('EventDB') IS NOT NULL
BEGIN
    ALTER DATABASE EventDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EventDB;
END;
GO
CREATE DATABASE EventDB COLLATE SQL_Latin1_General_CP1_CI_AI;
GO
USE EventDB;
GO

CREATE TABLE [User](
    UserId INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(100),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(15),
    Role VARCHAR(16) NOT NULL CHECK (Role IN ('Customer','Host','Admin')),
    WalletBalance DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (WalletBalance >= 0),
    CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2(0)
);

CREATE TABLE Event(
    EventId INT IDENTITY PRIMARY KEY,
    HostId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    StartTime DATETIME2(0) NOT NULL,
    EndTime DATETIME2(0) NOT NULL,
    Location NVARCHAR(200),
    Category NVARCHAR(100),
    Status VARCHAR(16) NOT NULL DEFAULT 'Draft' CHECK (Status IN ('Draft','Open','Closed','Cancelled')),
    CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2(0),
    CONSTRAINT FK_Event_Host FOREIGN KEY (HostId) REFERENCES [User](UserId)
);

CREATE TABLE TicketType(
    TicketTypeId INT IDENTITY PRIMARY KEY,
    EventId INT NOT NULL,
    TypeName NVARCHAR(50) NOT NULL,
    Price DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    Quantity INT NOT NULL CHECK (Quantity >= 0),
    MinOrder INT DEFAULT 1 CHECK (MinOrder >= 1),
    MaxOrder INT,
    SaleStart DATETIME2(0) NOT NULL,
    SaleEnd DATETIME2(0) NOT NULL,
    Status VARCHAR(16) NOT NULL DEFAULT 'Active' CHECK (Status IN ('Active','Inactive')),
    CONSTRAINT FK_TicketType_Event FOREIGN KEY (EventId) REFERENCES Event(EventId)
);

CREATE TABLE [Order](
    OrderId INT IDENTITY PRIMARY KEY,
    CustomerId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL CHECK (Amount >= 0),
    Status VARCHAR(16) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending','Paid','Failed','Cancelled','Refunded')),
    PaymentMethod VARCHAR(20),
    CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2(0),
    CONSTRAINT FK_Order_Customer FOREIGN KEY (CustomerId) REFERENCES [User](UserId)
);

CREATE TABLE OrderItem(
    OrderItemId INT IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL,
    TicketTypeId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    SeatNo NVARCHAR(50),
    Status VARCHAR(16) DEFAULT 'Reserved' CHECK (Status IN ('Reserved','Confirmed','Cancelled')),
    CONSTRAINT FK_OrderItem_Order FOREIGN KEY (OrderId) REFERENCES [Order](OrderId),
    CONSTRAINT FK_OrderItem_TicketType FOREIGN KEY (TicketTypeId) REFERENCES TicketType(TicketTypeId)
);

CREATE TABLE Ticket(
    TicketId INT IDENTITY PRIMARY KEY,
    TicketTypeId INT NOT NULL,
    OrderItemId INT,
    SerialNumber NVARCHAR(50) NOT NULL UNIQUE,
    Status VARCHAR(16) NOT NULL DEFAULT 'Available' CHECK (Status IN ('Available','Assigned','Used','Refunded')),
    IssuedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UsedAt DATETIME2(0),
    RefundedAt DATETIME2(0),
    CONSTRAINT FK_Ticket_TicketType FOREIGN KEY (TicketTypeId) REFERENCES TicketType(TicketTypeId),
    CONSTRAINT FK_Ticket_OrderItem FOREIGN KEY (OrderItemId) REFERENCES OrderItem(OrderItemId)
);

CREATE TABLE Payment(
    TransactionId INT IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL CHECK (Amount >= 0),
    Method VARCHAR(20) NOT NULL,
    Status VARCHAR(16) NOT NULL DEFAULT 'Initiated' CHECK (Status IN ('Initiated','Succeeded','Failed','Refunded')),
    PaymentDate DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Payment_Order FOREIGN KEY (OrderId) REFERENCES [Order](OrderId)
);

-- Wishlist table (simplified - direct relationship with User and TicketType)
CREATE TABLE Wishlist(
    Id INT IDENTITY PRIMARY KEY,
    UserId INT NOT NULL,
    TicketTypeId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1 CHECK (Quantity >= 1),
    AddedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2(0),
    CONSTRAINT FK_Wishlist_User FOREIGN KEY (UserId) REFERENCES [User](UserId) ON DELETE CASCADE,
    CONSTRAINT FK_Wishlist_TicketType FOREIGN KEY (TicketTypeId) REFERENCES TicketType(TicketTypeId),
    CONSTRAINT UQ_Wishlist_User_TicketType UNIQUE (UserId, TicketTypeId)
);

-- Voucher table for discount management
CREATE TABLE Voucher(
    VoucherId INT IDENTITY PRIMARY KEY,
    VoucherCode NVARCHAR(50) NOT NULL UNIQUE,
    DiscountPercentage INT NOT NULL CHECK (DiscountPercentage >= 0 AND DiscountPercentage <= 100),
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

-- Indexes

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
CREATE INDEX IX_Voucher_IsActive ON Voucher(IsActive);

