-- Chạy trong ChillCOFE_Dev sau khi sao lưu DB. Không chạy EF database update thay script này.
USE [ChillCOFE_Dev];
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
    THROW 50001, N'Không tìm thấy bảng dbo.Orders. Kiểm tra database.', 1;
IF OBJECT_ID(N'dbo.CafeCustomers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CafeCustomers (
        CafeCustomerId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CafeCustomers PRIMARY KEY,
        Name nvarchar(100) NOT NULL,
        Phone nvarchar(20) NOT NULL,
        Email nvarchar(200) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_CafeCustomers_IsActive DEFAULT 1,
        CreatedAt datetime2 NOT NULL CONSTRAINT DF_CafeCustomers_CreatedAt DEFAULT SYSDATETIME()
    );
END;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.CafeCustomers') AND name=N'IX_CafeCustomers_Phone')
    CREATE UNIQUE INDEX IX_CafeCustomers_Phone ON dbo.CafeCustomers(Phone);
IF COL_LENGTH(N'dbo.Orders', N'CafeCustomerId') IS NULL
    EXEC(N'ALTER TABLE dbo.Orders ADD CafeCustomerId int NULL;');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Orders_CafeCustomers')
    EXEC(N'ALTER TABLE dbo.Orders WITH CHECK ADD CONSTRAINT FK_Orders_CafeCustomers FOREIGN KEY (CafeCustomerId) REFERENCES dbo.CafeCustomers(CafeCustomerId);');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Orders') AND name=N'IX_Orders_CafeCustomerId')
    EXEC(N'CREATE INDEX IX_Orders_CafeCustomerId ON dbo.Orders(CafeCustomerId);');
COMMIT;
