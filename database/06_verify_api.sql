-- CHỈ ĐỌC. Thay khoảng ngày và ID theo kết quả API thực tế.
USE [ChillCOFE_Dev];
GO
DECLARE @From datetime2='2026-10-04T00:00:00', @To datetime2='2026-10-05T00:00:00';
SELECT COALESCE(SUM(CAST(TotalAmount AS decimal(18,2))),0) AS Revenue, COUNT(*) AS PaidOrders
FROM dbo.Orders WHERE IsPaid=1 AND Status=N'Đã thanh toán' AND OrderTime>=@From AND OrderTime<@To;
SELECT Status, IsPaid, COUNT(*) AS OrderCount FROM dbo.Orders
WHERE OrderTime>=@From AND OrderTime<@To GROUP BY Status, IsPaid;
SELECT TOP (5) d.ProductID, p.Name, SUM(CAST(d.Quantity AS bigint)) AS Quantity,
 SUM(CAST(d.Price AS decimal(18,2))*d.Quantity) AS GrossAmount
FROM dbo.OrderDetails d JOIN dbo.Orders o ON o.OrderID=d.OrderID
JOIN dbo.Products p ON p.ProductID=d.ProductID
WHERE o.IsPaid=1 AND o.Status=N'Đã thanh toán' AND o.OrderTime>=@From AND o.OrderTime<@To
GROUP BY d.ProductID,p.Name ORDER BY Quantity DESC,d.ProductID;
SELECT TOP(20) OrderID,TableID,CafeCustomerId,Status,IsPaid,TotalAmount,CustomerPaid,ChangeAmount,PaymentMethod
FROM dbo.Orders ORDER BY OrderID DESC;
SELECT TableID,Name,Status FROM dbo.Tables ORDER BY TableID;
