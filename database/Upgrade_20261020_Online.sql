-- Sao lưu database trước. Chạy Upgrade_20261014.sql trước file này.
-- Chạy trên database đang cấu hình ở ConnectionStrings:DefaultConnection.
USE [ChillCOFE_Dev];
GO
SET XACT_ABORT ON;
BEGIN TRY
 BEGIN TRANSACTION;
 IF OBJECT_ID(N'dbo.Orders',N'U') IS NULL OR OBJECT_ID(N'dbo.Products',N'U') IS NULL
  THROW 51000,N'Sai database hoặc thiếu schema gốc.',1;
 IF COL_LENGTH(N'dbo.Orders',N'IsOnline') IS NULL
  ALTER TABLE dbo.Orders ADD IsOnline bit NOT NULL CONSTRAINT DF_Orders_IsOnline DEFAULT(0);
 IF COL_LENGTH(N'dbo.Orders',N'PaidAt') IS NULL
  ALTER TABLE dbo.Orders ADD PaidAt datetime2 NULL;
 IF COL_LENGTH(N'dbo.Orders',N'DeliveredAt') IS NULL
  ALTER TABLE dbo.Orders ADD DeliveredAt datetime2 NULL;
 IF COL_LENGTH(N'dbo.Orders',N'CheckoutKey') IS NULL
  ALTER TABLE dbo.Orders ADD CheckoutKey nvarchar(36) NULL;
 IF COL_LENGTH(N'dbo.Tables',N'Note') IS NULL
  ALTER TABLE dbo.Tables ADD Note nvarchar(300) NULL;
 IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name=N'IX_Orders_CheckoutKey' AND object_id=OBJECT_ID(N'dbo.Orders'))
  EXEC(N'CREATE UNIQUE INDEX IX_Orders_CheckoutKey ON dbo.Orders(CheckoutKey) WHERE CheckoutKey IS NOT NULL');
 -- Nhận diện đơn online cũ, giữ dữ liệu tiền và không tự xác nhận đã giao.
 EXEC(N'UPDATE dbo.Orders SET IsOnline=1 WHERE Status=N''Online''; UPDATE dbo.Orders SET Status=N''Chờ xác nhận'' WHERE IsOnline=1 AND Status=N''Online'' AND IsPaid=0;');
 -- Chỉ thêm User cho tài khoản không có vai trò; không đổi quyền Staff/Admin.
 IF NOT EXISTS(SELECT 1 FROM dbo.AspNetRoles WHERE NormalizedName=N'USER')
  INSERT INTO dbo.AspNetRoles(Id,Name,NormalizedName,ConcurrencyStamp) VALUES(CONVERT(nvarchar(36),NEWID()),N'User',N'USER',CONVERT(nvarchar(36),NEWID()));
 INSERT INTO dbo.AspNetUserRoles(UserId,RoleId)
 SELECT u.Id,r.Id FROM dbo.AspNetUsers u CROSS JOIN dbo.AspNetRoles r
 WHERE r.NormalizedName=N'USER' AND NOT EXISTS(SELECT 1 FROM dbo.AspNetUserRoles ur WHERE ur.UserId=u.Id);
 COMMIT;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 THROW;
END CATCH;
GO
SELECT DB_NAME() AS DatabaseName, COLUMN_NAME,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME=N'Orders' AND COLUMN_NAME IN(N'IsOnline',N'PaidAt',N'DeliveredAt',N'CheckoutKey');
SELECT N'Nâng cấp online hoàn tất; không xóa dữ liệu cũ.' AS Result;
