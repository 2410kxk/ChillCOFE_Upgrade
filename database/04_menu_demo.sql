-- Demo names/prices. Does not modify existing order lines.
-- For a name already present: keep its price/category/availability; update image/description/customization.
USE [ChillCOFE_Dev];
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;
DECLARE @category int;
IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Cà phê') INSERT dbo.Categories(Name) VALUES(N'Cà phê');
SELECT TOP (1) @category=CategoryID FROM dbo.Categories WHERE Name=N'Cà phê' ORDER BY CategoryID;
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Cà phê đen')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Cà phê đen',20000,@category,N'/images/ca-phe-nong.jpg',N'Cà phê đậm vị, hậu đắng nhẹ, dành cho người thích sự nguyên bản.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/ca-phe-nong.jpg',Description=N'Cà phê đậm vị, hậu đắng nhẹ, dành cho người thích sự nguyên bản.',SupportsCustomization=1 WHERE Name=N'Cà phê đen';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Cà phê sữa')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Cà phê sữa',25000,@category,N'/images/ca-phe-sua-da-viet-nam.jpg',N'Cà phê Việt hòa cùng sữa đặc, thơm và béo vừa đủ.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/ca-phe-sua-da-viet-nam.jpg',Description=N'Cà phê Việt hòa cùng sữa đặc, thơm và béo vừa đủ.',SupportsCustomization=1 WHERE Name=N'Cà phê sữa';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Cà phê muối')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Cà phê muối',35000,@category,N'/images/ca-phe-kem-muoi.jpg',N'Cà phê đậm đà dưới lớp kem muối mềm mịn.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/ca-phe-kem-muoi.jpg',Description=N'Cà phê đậm đà dưới lớp kem muối mềm mịn.',SupportsCustomization=1 WHERE Name=N'Cà phê muối';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Bạc xỉu')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Bạc xỉu',30000,@category,N'/images/ly-ca-phe-bac-xiu-da.jpg',N'Nhiều sữa, chút cà phê, một lựa chọn nhẹ nhàng.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/ly-ca-phe-bac-xiu-da.jpg',Description=N'Nhiều sữa, chút cà phê, một lựa chọn nhẹ nhàng.',SupportsCustomization=1 WHERE Name=N'Bạc xỉu';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Cà phê dừa')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Cà phê dừa',39000,@category,N'/images/Ca-phe-dua-la-gi.png',N'Hương cà phê quyện cùng vị dừa thơm béo.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/Ca-phe-dua-la-gi.png',Description=N'Hương cà phê quyện cùng vị dừa thơm béo.',SupportsCustomization=1 WHERE Name=N'Cà phê dừa';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Americano')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Americano',35000,@category,N'/images/homemade-Iced-Americano-recipe_5.jpg',N'Vị cà phê rõ nét, thanh nhẹ và ít ngọt.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/homemade-Iced-Americano-recipe_5.jpg',Description=N'Vị cà phê rõ nét, thanh nhẹ và ít ngọt.',SupportsCustomization=1 WHERE Name=N'Americano';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Cappuccino')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Cappuccino',45000,@category,N'/images/como-hacer-un-capuchino-perfecto-maquina.webp',N'Cà phê và sữa với lớp bọt mịn, phục vụ theo khẩu phần chuẩn.',1,0);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/como-hacer-un-capuchino-perfecto-maquina.webp',Description=N'Cà phê và sữa với lớp bọt mịn, phục vụ theo khẩu phần chuẩn.',SupportsCustomization=0 WHERE Name=N'Cappuccino';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Black Orange')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Black Orange',42000,@category,N'/images/blackorange.jpg',N'Cà phê kết hợp cam, tươi mới và cân bằng.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/blackorange.jpg',Description=N'Cà phê kết hợp cam, tươi mới và cân bằng.',SupportsCustomization=1 WHERE Name=N'Black Orange';
IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Trà trái cây') INSERT dbo.Categories(Name) VALUES(N'Trà trái cây');
SELECT TOP (1) @category=CategoryID FROM dbo.Categories WHERE Name=N'Trà trái cây' ORDER BY CategoryID;
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Trà đào')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Trà đào',39000,@category,N'/images/tra dao.jpg',N'Trà thơm kết hợp đào, ngọt thanh và dễ uống.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/tra dao.jpg',Description=N'Trà thơm kết hợp đào, ngọt thanh và dễ uống.',SupportsCustomization=1 WHERE Name=N'Trà đào';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Trà táo quế')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Trà táo quế',39000,@category,N'/images/tra tao que.jpg',N'Hương táo dịu nhẹ hòa cùng nốt quế ấm áp.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/tra tao que.jpg',Description=N'Hương táo dịu nhẹ hòa cùng nốt quế ấm áp.',SupportsCustomization=1 WHERE Name=N'Trà táo quế';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Trà ô long')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Trà ô long',32000,@category,N'/images/tra olong.jpg',N'Trà ô long thơm nhẹ, hậu vị thanh.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/tra olong.jpg',Description=N'Trà ô long thơm nhẹ, hậu vị thanh.',SupportsCustomization=1 WHERE Name=N'Trà ô long';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Trà hoa cúc')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Trà hoa cúc',35000,@category,N'/images/tra hoa cuc.jpg',N'Hương hoa dịu nhẹ cho một khoảng nghỉ thư thái.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/tra hoa cuc.jpg',Description=N'Hương hoa dịu nhẹ cho một khoảng nghỉ thư thái.',SupportsCustomization=1 WHERE Name=N'Trà hoa cúc';
IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Trà sữa') INSERT dbo.Categories(Name) VALUES(N'Trà sữa');
SELECT TOP (1) @category=CategoryID FROM dbo.Categories WHERE Name=N'Trà sữa' ORDER BY CategoryID;
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Trà sữa ô long')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Trà sữa ô long',39000,@category,N'/images/tra sua olong.jpg',N'Ô long thơm quyện sữa, béo nhẹ và cân bằng.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/tra sua olong.jpg',Description=N'Ô long thơm quyện sữa, béo nhẹ và cân bằng.',SupportsCustomization=1 WHERE Name=N'Trà sữa ô long';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Trà sữa matcha')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Trà sữa matcha',45000,@category,N'/images/tra sua matcha.jpg',N'Hương trà xanh với sữa, đắng nhẹ và thơm mát.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/tra sua matcha.jpg',Description=N'Hương trà xanh với sữa, đắng nhẹ và thơm mát.',SupportsCustomization=1 WHERE Name=N'Trà sữa matcha';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Trà sữa dâu')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Trà sữa dâu',42000,@category,N'/images/trasuadau.jpg',N'Vị dâu ngọt dịu cùng sữa thơm.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/trasuadau.jpg',Description=N'Vị dâu ngọt dịu cùng sữa thơm.',SupportsCustomization=1 WHERE Name=N'Trà sữa dâu';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Trà sữa khoai môn')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Trà sữa khoai môn',42000,@category,N'/images/khoaimon.png',N'Khoai môn thơm béo, sắc tím nhẹ nhàng.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/khoaimon.png',Description=N'Khoai môn thơm béo, sắc tím nhẹ nhàng.',SupportsCustomization=1 WHERE Name=N'Trà sữa khoai môn';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Trà sữa socola')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Trà sữa socola',45000,@category,N'/images/trasuasocola.webp',N'Socola đậm vị hòa cùng sữa mềm mịn.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/trasuasocola.webp',Description=N'Socola đậm vị hòa cùng sữa mềm mịn.',SupportsCustomization=1 WHERE Name=N'Trà sữa socola';
IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Cacao') INSERT dbo.Categories(Name) VALUES(N'Cacao');
SELECT TOP (1) @category=CategoryID FROM dbo.Categories WHERE Name=N'Cacao' ORDER BY CategoryID;
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Cacao nóng')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Cacao nóng',35000,@category,N'/images/pha-cacaonong.png',N'Cacao ấm, đậm đà, phục vụ theo khẩu phần chuẩn.',1,0);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/pha-cacaonong.png',Description=N'Cacao ấm, đậm đà, phục vụ theo khẩu phần chuẩn.',SupportsCustomization=0 WHERE Name=N'Cacao nóng';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Cacao đá')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Cacao đá',39000,@category,N'/images/cacdaoda.jpg',N'Cacao đậm vị với sữa và đá mát lạnh.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/cacdaoda.jpg',Description=N'Cacao đậm vị với sữa và đá mát lạnh.',SupportsCustomization=1 WHERE Name=N'Cacao đá';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Cacao kem muối')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Cacao kem muối',45000,@category,N'/images/cacaomuoi.webp',N'Cacao thơm đậm dưới lớp kem muối béo nhẹ.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/cacaomuoi.webp',Description=N'Cacao thơm đậm dưới lớp kem muối béo nhẹ.',SupportsCustomization=1 WHERE Name=N'Cacao kem muối';
IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Soda') INSERT dbo.Categories(Name) VALUES(N'Soda');
SELECT TOP (1) @category=CategoryID FROM dbo.Categories WHERE Name=N'Soda' ORDER BY CategoryID;
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Soda dâu')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Soda dâu',35000,@category,N'/images/sodadau.jpg',N'Soda sảng khoái kết hợp vị dâu.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/sodadau.jpg',Description=N'Soda sảng khoái kết hợp vị dâu.',SupportsCustomization=1 WHERE Name=N'Soda dâu';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Soda kiwi')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Soda kiwi',35000,@category,N'/images/sodakiwi.jpg',N'Kiwi chua nhẹ và soda mát lạnh.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/sodakiwi.jpg',Description=N'Kiwi chua nhẹ và soda mát lạnh.',SupportsCustomization=1 WHERE Name=N'Soda kiwi';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Soda việt quất')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Soda việt quất',35000,@category,N'/images/pha-soda-viet-quat.jpg',N'Hương việt quất tươi mới cùng soda.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/pha-soda-viet-quat.jpg',Description=N'Hương việt quất tươi mới cùng soda.',SupportsCustomization=1 WHERE Name=N'Soda việt quất';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Soda chanh dây')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Soda chanh dây',35000,@category,N'/images/soda_chanh_day_6_3754d11c5b.jpg',N'Chanh dây chua dịu, thơm và sảng khoái.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/soda_chanh_day_6_3754d11c5b.jpg',Description=N'Chanh dây chua dịu, thơm và sảng khoái.',SupportsCustomization=1 WHERE Name=N'Soda chanh dây';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Soda blue')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Soda blue',35000,@category,N'/images/sodablue.webp',N'Một ly soda xanh mát cho ngày nhiều năng lượng.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/sodablue.webp',Description=N'Một ly soda xanh mát cho ngày nhiều năng lượng.',SupportsCustomization=1 WHERE Name=N'Soda blue';
IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Nước ép & sinh tố') INSERT dbo.Categories(Name) VALUES(N'Nước ép & sinh tố');
SELECT TOP (1) @category=CategoryID FROM dbo.Categories WHERE Name=N'Nước ép & sinh tố' ORDER BY CategoryID;
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Nước ép cam')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Nước ép cam',39000,@category,N'/images/nuoc ep cam.jpg',N'Vị cam tươi mát, chua ngọt cân bằng.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/nuoc ep cam.jpg',Description=N'Vị cam tươi mát, chua ngọt cân bằng.',SupportsCustomization=1 WHERE Name=N'Nước ép cam';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Nước ép dưa hấu')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Nước ép dưa hấu',35000,@category,N'/images/nuoc ep dua hau.jpg',N'Dưa hấu ngọt thanh, nhẹ nhàng và mát lành.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/nuoc ep dua hau.jpg',Description=N'Dưa hấu ngọt thanh, nhẹ nhàng và mát lành.',SupportsCustomization=1 WHERE Name=N'Nước ép dưa hấu';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Nước ép dứa')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Nước ép dứa',35000,@category,N'/images/nuoc ep dua.jpg',N'Hương dứa nhiệt đới, thơm và chua dịu.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/nuoc ep dua.jpg',Description=N'Hương dứa nhiệt đới, thơm và chua dịu.',SupportsCustomization=1 WHERE Name=N'Nước ép dứa';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Nước ép táo')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Nước ép táo',39000,@category,N'/images/nuoc ep tao.jpg',N'Táo thơm nhẹ với vị ngọt thanh.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/nuoc ep tao.jpg',Description=N'Táo thơm nhẹ với vị ngọt thanh.',SupportsCustomization=1 WHERE Name=N'Nước ép táo';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Sinh tố xoài')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Sinh tố xoài',45000,@category,N'/images/sinh to xoai.jpg',N'Xoài thơm ngọt, xay mịn cùng sữa.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/sinh to xoai.jpg',Description=N'Xoài thơm ngọt, xay mịn cùng sữa.',SupportsCustomization=1 WHERE Name=N'Sinh tố xoài';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Sinh tố dâu')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Sinh tố dâu',45000,@category,N'/images/sinh to dau.jpg',N'Dâu chua ngọt, sánh mịn và mát lạnh.',1,1);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/sinh to dau.jpg',Description=N'Dâu chua ngọt, sánh mịn và mát lạnh.',SupportsCustomization=1 WHERE Name=N'Sinh tố dâu';
IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name=N'Bánh ngọt') INSERT dbo.Categories(Name) VALUES(N'Bánh ngọt');
SELECT TOP (1) @category=CategoryID FROM dbo.Categories WHERE Name=N'Bánh ngọt' ORDER BY CategoryID;
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Tiramisu')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Tiramisu',49000,@category,N'/images/banhtiramisu.jpg',N'Bánh vị cà phê và cacao, dùng kèm đồ uống yêu thích.',1,0);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/banhtiramisu.jpg',Description=N'Bánh vị cà phê và cacao, dùng kèm đồ uống yêu thích.',SupportsCustomization=0 WHERE Name=N'Tiramisu';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Cheesecake')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Cheesecake',45000,@category,N'/images/banhcheesecake.jpg',N'Bánh phô mai mềm mịn, chua béo nhẹ.',1,0);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/banhcheesecake.jpg',Description=N'Bánh phô mai mềm mịn, chua béo nhẹ.',SupportsCustomization=0 WHERE Name=N'Cheesecake';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Croissant')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Croissant',29000,@category,N'/images/banhcroissant.jpg',N'Bánh sừng bò thơm bơ, lớp vỏ giòn nhẹ.',1,0);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/banhcroissant.jpg',Description=N'Bánh sừng bò thơm bơ, lớp vỏ giòn nhẹ.',SupportsCustomization=0 WHERE Name=N'Croissant';
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name=N'Mousse socola')
 INSERT dbo.Products(Name,Price,CategoryID,ImageUrl,Description,IsAvailable,SupportsCustomization)
 VALUES(N'Mousse socola',45000,@category,N'/images/banhmoussesocola.jpg',N'Mousse socola mềm, đậm vị cacao.',1,0);
ELSE
 UPDATE dbo.Products SET ImageUrl=N'/images/banhmoussesocola.jpg',Description=N'Mousse socola mềm, đậm vị cacao.',SupportsCustomization=0 WHERE Name=N'Mousse socola';
COMMIT TRANSACTION;
GO
SELECT COUNT(*) AS TotalProducts FROM dbo.Products;
