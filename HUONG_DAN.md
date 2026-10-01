# Chill COFÉ — Nâng cấp giao diện & POS

Bản nâng cấp dựa trực tiếp vào src.rar + images.rar bạn gửi ngày 01/10/2026. Giữ ASP.NET Core MVC net8.0, namespace và UserSecretsId cũ. Không phải sản phẩm đã nghiệm thu. Bản tuần 4 trước đã được bạn build/chạy thành công; bản nâng cấp này chưa được build hoặc chạy SQL Server tại môi trường tạo gói.

## Có gì mới
- Nhận diện kem–olive, trang chủ, menu ảnh, tìm kiếm không dấu, lọc danh mục và sắp xếp giá.
- 35 món demo = 31 đồ uống + 4 bánh, trong 7 danh mục; sử dụng ảnh bạn cung cấp. Tên/giá/mô tả là mẫu để nhóm điều chỉnh.
- Bàn trống/đang dùng; POS cùng màn hình chọn món và giỏ hàng; responsive desktop/mobile bằng CSS.
- Size M/L (+7.000đ), đường 0/30/50/70/100%, đá 0/50/100%, topping trân châu/thạch +5.000đ, kem cheese +10.000đ; ghi chú tối đa 200 ký tự.
- Món không hỗ trợ tùy chọn (bánh, một số món nóng) dùng giá tiêu chuẩn. Giá và tùy chọn được kiểm tra ở server; không lấy tổng tiền từ trình duyệt làm số tiền thu.
- Tiền mặt: kiểm tra đủ tiền và lưu tiền thừa. Chuyển khoản: xác nhận thủ công đã nhận tiền, CHƯA tích hợp ngân hàng/cổng thanh toán.
- Phiếu thanh toán có thể in / lưu PDF từ trình duyệt. Đây là phiếu nội bộ, không phải hóa đơn điện tử thuế hay báo cáo PDF tổng hợp.
- Quản lý món: thêm/sửa, tải ảnh, mô tả và bật/tắt tùy chọn; báo tạm hết/mở bán lại, giữ lịch sử đơn.
- Checkout ràng buộc đúng mã đơn và tổng tiền người thu ngân đang xem, kiểm tra trạng thái trong transaction. Cần thử đồng thời trên SQL Server thực tế.

## Cập nhật trên máy bạn — theo đúng thứ tự
1. Dừng web bằng Ctrl+C. Sao lưu thư mục ChillCOFE_Week4 và database ChillCOFE_Dev. Trong SSMS có thể chọn database → Tasks → Back Up. Nếu bản LocalDB của bạn không có menu, dùng công cụ backup phù hợp trước khi cập nhật.
2. Giải nén gói này vào thư mục mới, ví dụ `D:\QLDA CNTT\ChillCOFE_Upgrade`. Không cần chép đè src cũ. Hai bản dùng chung UserSecretsId nên giữ được cấu hình kết nối/tài khoản trên cùng Windows user.
3. Trong SSMS kết nối `(localdb)\MSSQLLocalDB`, mở và chạy `database/03_upgrade_schema.sql`, sau đó `database/04_menu_demo.sql`. Cả hai nhắm ChillCOFE_Dev; không chạy lại 01_schema.sql. Các script có thể chạy lại, không xóa bảng hoặc đơn. 04 cập nhật ảnh/mô tả/cờ tùy chọn của món trùng tên nhưng giữ giá, danh mục và trạng thái bán đã có. Nếu bạn đã chỉnh mô tả/ảnh các món trùng tên thì cần review trước vì chúng sẽ được thay bằng mẫu mới.
4. Mở thư mục mới trong VS Code; Terminal PowerShell chạy:

```powershell
Set-Location "D:\QLDA CNTT\ChillCOFE_Upgrade\src"
dotnet restore
dotnet build
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --no-launch-profile --urls "https://localhost:7043"
```

5. Mở https://localhost:7043 và nhấn Ctrl+F5 để tải CSS mới. Dùng tài khoản Admin/Staff đã tạo. Menu mới /Cafe/Menu; bàn /Pos/Tables; quản trị /CatalogAdmin; doanh thu /Statistics/Revenue.
6. Chạy kiểm tra trong docs/KIEM_TRA.md. Nếu gặp `Invalid column name IsAvailable/Size/...`, kiểm tra đã chạy 03 vào đúng database của connection string. Nếu cổng 7043 bận, dừng tiến trình web cũ trước khi chạy bản mới. Nếu lỗi build, gửi toàn bộ error (không chỉ warning).

Không chạy `dotnet ef database update`: migration kế thừa chưa được baseline với schema nhập từ SQL. Bản này dùng script SQL bổ sung. KHÔNG bật lại Seed nếu đã có tài khoản/demo; script menu độc lập với Seed.

## Xem trước
Mở XEM_TRUOC.html sau khi giải nén để xem trang chủ/menu với ảnh và CSS thực, không cần server. Đây là bản tĩnh; chưa có các chức năng của MVC. Chức năng chỉ xác minh bằng chạy web và kiểm tra dữ liệu.

## Kiểm thử và giới hạn
- Đã kiểm tra: cú pháp JavaScript bằng node --check; cấu trúc XML project/JSON, đủ ảnh tham chiếu 35 món, tên cột model/script, endpoint và ZIP integrity.
- Chưa kiểm tra được: dotnet restore/build, Razor compilation, SQL migration, ứng dụng với database thật, thanh toán đồng thời, trình bày bằng trình duyệt thực. Môi trường không có .NET SDK/SQL Server; Playwright có thư viện nhưng thiếu Chromium. Không tuyên bố các test nghiệp vụ đã Pass.
- Tiền trong DB vẫn giữ double/float kế thừa để tránh chuyển kiểu gây ảnh hưởng dữ liệu. Phép tính tổng POS dùng decimal rồi lưu lại theo schema hiện có. Cần migration decimal toàn hệ thống ở giai đoạn tiếp theo.
- Tùy chọn lịch sử của đơn cũ được gán mặc định M/100%/không topping; không thể khôi phục lựa chọn không được lưu trước đây. Tên món trên phiếu lấy tên sản phẩm hiện tại, chưa có snapshot tên tại thời điểm bán.
- Phần mua online kế thừa không nằm trong luồng UI mới và chưa được nâng cấp/kiểm thử. Giai đoạn này tập trung bán hàng tại quầy; tránh đánh dấu online checkout Done.
- Chưa bổ sung loyalty, import/export Excel, báo cáo tổng hợp PDF/Excel, bếp/notification, đặt bàn, AI hoặc triển khai internet. Không đánh dấu toàn bộ dự án hoàn thành.
- Danh mục hiện do SQL demo tạo; giao diện CRUD danh mục riêng chưa có. Endpoint cũ sửa đơn tại bàn được ngừng sử dụng để tránh bỏ qua checkout mới; link chọn bàn/menu cũ chuyển sang POS mới.
- Quản lý món có giới hạn ảnh; giữ các chức năng quản trị kế thừa qua menu. Chưa thực hiện kiểm toán bảo mật toàn bộ controller cũ.

## Quay lại bản cũ nếu cần
Dừng bản mới, mở lại thư mục nguồn tuần 4 và chạy trên cùng database. Cột mới có default nên mã cũ có thể tiếp tục tạo dữ liệu cơ bản, nhưng không hiểu topping/size. Không dùng mã cũ để sửa/thanh toán đơn tùy chọn đang mở. Nếu cần quay lại dữ liệu nguyên trạng, restore bản backup vào DB riêng và đổi kết nối; việc này sẽ mất các thay đổi phát sinh sau backup trên bản khôi phục. Không có script DROP cột/tables trong gói này.
