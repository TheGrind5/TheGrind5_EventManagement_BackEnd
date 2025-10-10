IF DB_ID('EventDB') IS NOT NULL
BEGIN
    ALTER DATABASE EventDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EventDB;
END;
GO
CREATE DATABASE EventDB;
GO
USE EventDB;
GO

CREATE TABLE [User](
    UserID INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(100),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(15),
    Role VARCHAR(16) NOT NULL CHECK (Role IN ('Customer','Host','Admin')),
    CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2(0)
);

CREATE TABLE Event(
    EventID INT IDENTITY PRIMARY KEY,
    HostID INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    StartTime DATETIME2(0) NOT NULL,
    EndTime DATETIME2(0) NOT NULL,
    Location NVARCHAR(200),
    Category NVARCHAR(100),
    Status VARCHAR(16) NOT NULL DEFAULT 'Draft' CHECK (Status IN ('Draft','Open','Closed','Cancelled')),
    CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2(0),
    CONSTRAINT FK_Event_Host FOREIGN KEY (HostID) REFERENCES [User](UserID)
);

CREATE TABLE TicketType(
    TicketTypeID INT IDENTITY PRIMARY KEY,
    EventID INT NOT NULL,
    TypeName NVARCHAR(50) NOT NULL,
    Price DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    Quantity INT NOT NULL CHECK (Quantity >= 0),
    MinOrder INT DEFAULT 1 CHECK (MinOrder >= 1),
    MaxOrder INT,
    SaleStart DATETIME2(0) NOT NULL,
    SaleEnd DATETIME2(0) NOT NULL,
    Status VARCHAR(16) NOT NULL DEFAULT 'Active' CHECK (Status IN ('Active','Inactive')),
    CONSTRAINT FK_TicketType_Event FOREIGN KEY (EventID) REFERENCES Event(EventID)
);

CREATE TABLE [Order](
    OrderID INT IDENTITY PRIMARY KEY,
    CustomerID INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL CHECK (Amount >= 0),
    Status VARCHAR(16) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending','Paid','Failed','Cancelled','Refunded')),
    PaymentMethod VARCHAR(20),
    CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2(0),
    CONSTRAINT FK_Order_Customer FOREIGN KEY (CustomerID) REFERENCES [User](UserID)
);

CREATE TABLE OrderItem(
    OrderItemID INT IDENTITY PRIMARY KEY,
    OrderID INT NOT NULL,
    TicketTypeID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    SeatNo NVARCHAR(50),
    Status VARCHAR(16) DEFAULT 'Reserved' CHECK (Status IN ('Reserved','Confirmed','Cancelled')),
    CONSTRAINT FK_OrderItem_Order FOREIGN KEY (OrderID) REFERENCES [Order](OrderID),
    CONSTRAINT FK_OrderItem_TicketType FOREIGN KEY (TicketTypeID) REFERENCES TicketType(TicketTypeID)
);

CREATE TABLE Ticket(
    TicketID INT IDENTITY PRIMARY KEY,
    TicketTypeID INT NOT NULL,
    OrderItemID INT,
    SerialNumber NVARCHAR(50) NOT NULL UNIQUE,
    Status VARCHAR(16) NOT NULL DEFAULT 'Available' CHECK (Status IN ('Available','Assigned','Used','Refunded')),
    IssuedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    UsedAt DATETIME2(0),
    RefundedAt DATETIME2(0),
    CONSTRAINT FK_Ticket_TicketType FOREIGN KEY (TicketTypeID) REFERENCES TicketType(TicketTypeID),
    CONSTRAINT FK_Ticket_OrderItem FOREIGN KEY (OrderItemID) REFERENCES OrderItem(OrderItemID)
);

CREATE TABLE Payment(
    TransactionID INT IDENTITY PRIMARY KEY,
    OrderID INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL CHECK (Amount >= 0),
    Method VARCHAR(20) NOT NULL,
    Status VARCHAR(16) NOT NULL DEFAULT 'Initiated' CHECK (Status IN ('Initiated','Succeeded','Failed','Refunded')),
    PaymentDate DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Payment_Order FOREIGN KEY (OrderID) REFERENCES [Order](OrderID)
);

-- Indexes
CREATE INDEX IX_Event_HostID ON Event(HostID);
CREATE INDEX IX_TicketType_EventID ON TicketType(EventID);
CREATE INDEX IX_Order_CustomerID ON [Order](CustomerID);
CREATE INDEX IX_OrderItem_OrderID ON OrderItem(OrderID);
CREATE INDEX IX_OrderItem_TicketTypeID ON OrderItem(TicketTypeID);
CREATE INDEX IX_Ticket_TicketTypeID ON Ticket(TicketTypeID);
CREATE INDEX IX_Ticket_OrderItemID ON Ticket(OrderItemID);
CREATE INDEX IX_Payment_OrderID ON Payment(OrderID);
