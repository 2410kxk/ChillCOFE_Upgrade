-- Chỉ đọc, dùng làm minh chứng sau khi chạy Upgrade_20261014.sql.
USE [ChillCOFE_Dev];
GO
SELECT DB_NAME() AS DatabaseName;
SELECT t.name AS TableName,c.name AS ColumnName,TYPE_NAME(c.user_type_id) AS DataType,c.max_length,c.is_nullable
FROM sys.tables t JOIN sys.columns c ON c.object_id=t.object_id
WHERE t.name IN(N'Reviews',N'ReviewComments',N'UserProfiles',N'CafeNotifications',N'NotificationReads',N'StoreSettings')
   OR(t.name=N'Tables' AND c.name=N'Note')
ORDER BY t.name,c.column_id;
SELECT N'Products' AS Entity,COUNT_BIG(*) AS Total FROM dbo.Products
UNION ALL SELECT N'Tables',COUNT_BIG(*) FROM dbo.Tables
UNION ALL SELECT N'Orders',COUNT_BIG(*) FROM dbo.Orders
UNION ALL SELECT N'AspNetUsers',COUNT_BIG(*) FROM dbo.AspNetUsers
UNION ALL SELECT N'Reviews',COUNT_BIG(*) FROM dbo.Reviews
UNION ALL SELECT N'ReviewComments',COUNT_BIG(*) FROM dbo.ReviewComments
UNION ALL SELECT N'UserProfiles',COUNT_BIG(*) FROM dbo.UserProfiles
UNION ALL SELECT N'CafeNotifications',COUNT_BIG(*) FROM dbo.CafeNotifications
UNION ALL SELECT N'NotificationReads',COUNT_BIG(*) FROM dbo.NotificationReads;
-- Chỉ kiểm tra cấu trúc/tổng số; không in mật khẩu, email hay nội dung khách hàng.
