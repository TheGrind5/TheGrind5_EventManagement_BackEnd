-- Tạo bảng OrderProduct
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OrderProduct' AND xtype='U')
BEGIN
    CREATE TABLE OrderProduct (
        OrderProductId int IDENTITY(1,1) PRIMARY KEY,
        OrderId int NOT NULL,
        ProductId int NOT NULL,
        Quantity int NOT NULL,
        Price decimal(18,2) NOT NULL,
        TotalPrice decimal(18,2) NOT NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY (OrderId) REFERENCES [Order](OrderId),
        FOREIGN KEY (ProductId) REFERENCES Product(ProductId)
    );
    
    PRINT 'Table OrderProduct created successfully';
END
ELSE
BEGIN
    PRINT 'Table OrderProduct already exists';
END
