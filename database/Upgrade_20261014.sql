-- Chill COFÉ: chạy trên database phát triển hiện có, sau khi sao lưu.
-- Không chạy lại sqlnew.sql, không xóa hay tạo lại database.
USE [ChillCOFE_Dev];
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.AspNetUsers',N'U') IS NULL OR OBJECT_ID(N'dbo.Tables',N'U') IS NULL
    THROW 51000, N'Chưa có schema gốc. Chọn đúng database ChillCOFE_Dev.', 1;
IF COL_LENGTH(N'dbo.Tables',N'Note') IS NULL
    ALTER TABLE dbo.Tables ADD Note nvarchar(max) NULL;
IF OBJECT_ID(N'dbo.Reviews',N'U') IS NULL
    CREATE TABLE dbo.Reviews(
        ReviewID int IDENTITY PRIMARY KEY, UserId nvarchar(450) NOT NULL,
        Content nvarchar(max) NOT NULL, Rating int NOT NULL,
        ImageUrl nvarchar(max) NULL, CreatedTime datetime NOT NULL,
        CONSTRAINT FK_Community_Reviews_User FOREIGN KEY(UserId) REFERENCES dbo.AspNetUsers(Id));
IF OBJECT_ID(N'dbo.ReviewComments',N'U') IS NULL
    CREATE TABLE dbo.ReviewComments(
        CommentID int IDENTITY PRIMARY KEY, ReviewID int NOT NULL,
        UserId nvarchar(450) NOT NULL, Content nvarchar(max) NOT NULL, CreatedTime datetime NOT NULL,
        CONSTRAINT FK_Community_Comments_Review FOREIGN KEY(ReviewID) REFERENCES dbo.Reviews(ReviewID) ON DELETE CASCADE,
        CONSTRAINT FK_Community_Comments_User FOREIGN KEY(UserId) REFERENCES dbo.AspNetUsers(Id));
IF OBJECT_ID(N'dbo.UserProfiles',N'U') IS NULL
    CREATE TABLE dbo.UserProfiles(
        UserId nvarchar(450) PRIMARY KEY, DisplayName nvarchar(80) NOT NULL,
        AvatarUrl nvarchar(max) NULL,
        CONSTRAINT FK_Community_Profile_User FOREIGN KEY(UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE);
IF OBJECT_ID(N'dbo.CafeNotifications',N'U') IS NULL
    CREATE TABLE dbo.CafeNotifications(
        Id int IDENTITY PRIMARY KEY, Title nvarchar(120) NOT NULL,
        Content nvarchar(4000) NOT NULL, Audience nvarchar(10) NOT NULL,
        AuthorId nvarchar(450) NOT NULL, CreatedAt datetime2 NOT NULL,
        CONSTRAINT CK_Community_Notification_Audience CHECK(Audience IN(N'All',N'User',N'Staff')),
        CONSTRAINT FK_Community_Notification_Author FOREIGN KEY(AuthorId) REFERENCES dbo.AspNetUsers(Id));
IF OBJECT_ID(N'dbo.NotificationReads',N'U') IS NULL
    CREATE TABLE dbo.NotificationReads(
        NotificationId int NOT NULL, UserId nvarchar(450) NOT NULL, ReadAt datetime2 NOT NULL,
        CONSTRAINT PK_Community_NotificationReads PRIMARY KEY(NotificationId,UserId),
        CONSTRAINT FK_Community_Read_Notification FOREIGN KEY(NotificationId) REFERENCES dbo.CafeNotifications(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Community_Read_User FOREIGN KEY(UserId) REFERENCES dbo.AspNetUsers(Id));
IF OBJECT_ID(N'dbo.StoreSettings',N'U') IS NULL
    CREATE TABLE dbo.StoreSettings(
        Id int IDENTITY PRIMARY KEY,EnableKitchenPrinter bit NOT NULL DEFAULT 0,
        EnableReceiptPrinter bit NOT NULL DEFAULT 0, EnableNotificationSound bit NOT NULL DEFAULT 0,
        StoreName nvarchar(max) NOT NULL,Address nvarchar(max) NOT NULL,Phone nvarchar(max) NOT NULL,
        WifiPassword nvarchar(max) NULL);
IF COL_LENGTH(N'dbo.StoreSettings',N'Email') IS NULL
    ALTER TABLE dbo.StoreSettings ADD Email nvarchar(150) NULL;
IF COL_LENGTH(N'dbo.StoreSettings',N'OpeningHours') IS NULL
    ALTER TABLE dbo.StoreSettings ADD OpeningHours nvarchar(150) NULL;
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name=N'CK_Community_Review_Rating')
   AND NOT EXISTS(SELECT 1 FROM dbo.Reviews WHERE Rating NOT BETWEEN 1 AND 5)
    ALTER TABLE dbo.Reviews ADD CONSTRAINT CK_Community_Review_Rating CHECK(Rating BETWEEN 1 AND 5);
COMMIT;
SELECT DB_NAME() AS DatabaseName, N'Nâng cấp schema hoàn tất. Không thay đổi tài khoản hay dữ liệu cũ.' AS Result;
SELECT name FROM sys.tables WHERE name IN(N'Reviews',N'ReviewComments',N'UserProfiles',N'CafeNotifications',N'NotificationReads',N'StoreSettings',N'Tables');
