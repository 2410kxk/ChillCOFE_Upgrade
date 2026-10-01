-- Chill COFÉ UI/POS upgrade. Run on ChillCOFE_Dev after taking a backup.
-- Additive, rerunnable: no table/order deletion and no price conversion.
USE [ChillCOFE_Dev];
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH(N'dbo.Products', N'IsAvailable') IS NULL
    ALTER TABLE dbo.[Products] ADD [IsAvailable] bit NOT NULL CONSTRAINT DF_Chill_Products_Available DEFAULT (1) WITH VALUES;
IF COL_LENGTH(N'dbo.Products', N'SupportsCustomization') IS NULL
    ALTER TABLE dbo.[Products] ADD [SupportsCustomization] bit NOT NULL CONSTRAINT DF_Chill_Products_Custom DEFAULT (1) WITH VALUES;
IF COL_LENGTH(N'dbo.OrderDetails', N'Size') IS NULL
    ALTER TABLE dbo.[OrderDetails] ADD [Size] nvarchar(1) NOT NULL CONSTRAINT DF_Chill_Detail_Size DEFAULT (N'M') WITH VALUES;
IF COL_LENGTH(N'dbo.OrderDetails', N'Sugar') IS NULL
    ALTER TABLE dbo.[OrderDetails] ADD [Sugar] int NOT NULL CONSTRAINT DF_Chill_Detail_Sugar DEFAULT (100) WITH VALUES;
IF COL_LENGTH(N'dbo.OrderDetails', N'Ice') IS NULL
    ALTER TABLE dbo.[OrderDetails] ADD [Ice] int NOT NULL CONSTRAINT DF_Chill_Detail_Ice DEFAULT (100) WITH VALUES;
IF COL_LENGTH(N'dbo.OrderDetails', N'Topping') IS NULL
    ALTER TABLE dbo.[OrderDetails] ADD [Topping] nvarchar(40) NOT NULL CONSTRAINT DF_Chill_Detail_Topping DEFAULT (N'Không') WITH VALUES;
IF COL_LENGTH(N'dbo.OrderDetails', N'Note') IS NULL
    ALTER TABLE dbo.[OrderDetails] ADD [Note] nvarchar(200) NOT NULL CONSTRAINT DF_Chill_Detail_Note DEFAULT (N'') WITH VALUES;
COMMIT TRANSACTION;
GO
