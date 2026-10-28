-- Chỉ áp dụng cho database phát triển mới, không áp dụng cho DB thật.
USE [ChillCOFE_Dev];
GO
-- Tài khoản/mật khẩu do ASP.NET Identity quản lý. Model nhân viên cũ không có Password.
ALTER TABLE [dbo].[Employees] ALTER COLUMN [Password] nvarchar(max) NULL;
ALTER TABLE [dbo].[Employees] ALTER COLUMN [Email] nvarchar(max) NULL;
GO
