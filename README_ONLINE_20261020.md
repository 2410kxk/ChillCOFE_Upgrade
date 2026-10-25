# Chill COFÉ — Nâng cấp đặt món và giao hàng 20/10/2026

## Cài đặt trên máy Windows

1. Dừng ứng dụng (Ctrl+C). Sao lưu thư mục dự án và database ChillCOFE_Dev trong SSMS.
2. Giải nén gói vào thư mục mới, không đè bản đang sử dụng. Gói có đầy đủ ảnh hiện có; không cần sao chép riêng thư mục ảnh. Nếu phát sinh upload mới sau bản ZIP đã gửi, sao chép thêm các file upload đó từ bản đang dùng.
3. Trong SSMS, kết nối đúng SQL Server/LocalDB của ứng dụng. Chạy lần lượt **database/Upgrade_20261014.sql**, sau đó **database/Upgrade_20261020_Online.sql**. Cả hai script mặc định dùng ChillCOFE_Dev; nếu dùng tên khác, sửa dòng USE trước khi chạy. Không chạy lại script tạo database gốc, không xóa database.
4. Nếu Messages có lỗi đỏ, dừng lại và lưu thông báo lỗi. Không chạy web mới khi schema chưa nâng cấp đủ.
5. Mở terminal tại thư mục gốc vừa giải nén và chạy:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=ChillCOFE_Dev;Trusted_Connection=True;TrustServerCertificate=True;" --project ".\src\QuanLiQuanCaffee.csproj"
dotnet user-secrets set "Seed:Enabled" "false" --project ".\src\QuanLiQuanCaffee.csproj"
dotnet build ".\src\QuanLiQuanCaffee.csproj"
dotnet run --project ".\src\QuanLiQuanCaffee.csproj" --no-launch-profile --urls "https://localhost:7043"
```

Dùng .NET 8 SDK hoặc SDK mới hơn hỗ trợ target net8.0. Giữ múi giờ Windows UTC+7 để các mốc bán hàng khớp giờ Việt Nam. Đăng xuất/đăng nhập lại tài khoản cũ sau khi bổ sung vai trò.

## Tính năng

- User: ô tìm món trên nav, nút giỏ hàng kèm tổng số lượng; thêm món hiện toast góc phải.
- Giỏ hàng: tăng/giảm/xóa, size M/L, topping, đường và đá (0/25/50/75/100/125%), ghi chú tối đa 200 ký tự. “Bình thường” = 100%. Nhấn **Lưu tùy chọn** sau khi sửa.
- Các món không hỗ trợ tùy chọn giữ size M, đường/đá 100%, không topping.
- Size L +7.000đ; trân châu/thạch +5.000đ; kem cheese +10.000đ, giữ quy tắc giá hiện có.
- Đặt hàng: tên, số điện thoại, địa chỉ, tiền mặt/chuyển khoản. Giá được kiểm tra lại trên máy chủ; món tạm hết không được đặt.
- Lịch sử chỉ hiển thị đơn của tài khoản đang đăng nhập. Giỏ hàng tách theo tài khoản, lưu trong phiên 30 phút; khởi động lại server/hết phiên có thể mất giỏ chưa đặt. Đơn đã đặt lưu database.
- Staff/Admin: **Đơn giao hàng** → Chờ xác nhận → Bắt đầu giao → xác nhận đã giao và nhận đủ tiền → Giao hàng thành công. Có thể hủy đơn chưa thanh toán.
- Giao thành công đặt IsPaid=true, Status="Đã thanh toán", PaidAt/DeliveredAt; báo cáo và danh sách đơn dùng cùng bản ghi, không tạo hóa đơn doanh thu thứ hai. Nút lặp không cộng doanh thu thêm.
- Doanh thu mới theo ngày thu tiền PaidAt. Đơn cũ thiếu PaidAt giữ ngày OrderTime làm mốc tương thích; không tự bịa ngày thu tiền cũ.
- Tất cả đăng ký mới gán User. Script chỉ bổ sung User cho tài khoản chưa có vai trò, giữ nguyên Staff/Admin.
- Sửa báo hết/mở bán lại bằng giá trị boolean rõ ràng và kiểm tra đầu vào.
- Trang chủ bỏ dải trượt trên cùng, ảnh cà phê bên phải, chữ bên trái; thêm câu chuyện, trải nghiệm và hiệu ứng hiện khi cuộn (tôn trọng cài đặt giảm chuyển động).
- Sơ đồ bàn dạng thẻ theo mẫu, lọc trạng thái/khu, Staff/Admin sửa ghi chú trực tiếp. Bàn đặt tên A01/B01/C01/D01 được nhóm vào khu tương ứng; tên khác thuộc Khu chung. Không tự đổi tên, thêm bàn hoặc đổi trạng thái dữ liệu cũ.

## Các giả định cần nhóm chốt

- **Phí giao hàng hiện là 0đ**; chưa có biểu phí/khoảng cách nên chưa tính phí vận chuyển.
- Chuyển khoản là phương thức ghi nhận và đối chiếu thủ công; chưa có tài khoản ngân hàng, QR hay cổng thanh toán. Chọn phương thức không tự ghi nhận đã nhận tiền.
- Database hiện có trạng thái bàn Trống/Đang dùng. Chưa tự thêm các luồng “Món đã ra/Đã đặt/Đã thanh toán” chỉ để giống màu ảnh; cần xác định nghiệp vụ bếp, đặt bàn và giải phóng bàn trước khi mở rộng các trạng thái này.
- Đơn online cũ không có UserId không thể tự gán vào lịch sử của một khách; cần đối soát và liên kết có căn cứ.
- Không tích hợp đơn vị vận chuyển bên ngoài; trạng thái giao hàng do Staff/Admin cập nhật. Danh sách có nút làm mới, không đẩy realtime.

## Kiểm tra trên SQL Server của nhóm

1. Đăng ký tài khoản mới; kiểm tra quyền User và thanh tìm kiếm/giỏ hàng.
2. Admin báo hết một món rồi mở lại; User thêm lại được, món hết không đặt được.
3. User chọn 2 món với size/topping khác nhau, kiểm tra giá và lưu tùy chọn; thử mở tài khoản khác để xác nhận giỏ/lịch sử riêng.
4. Đặt đơn, ghi lại mã; kiểm tra Orders, OrderDetails và lịch sử có đủ tùy chọn, thông tin giao hàng.
5. Staff chuyển sang Đang giao: doanh thu chưa tăng. Xác nhận đủ tiền/giao thành công: doanh thu tăng đúng một lần; bấm lại không tăng thêm.
6. Thử User gọi các thao tác Staff: phải bị từ chối. Thử hủy đơn đã thanh toán: không thay đổi dữ liệu.
7. Sửa ghi chú bàn bằng Staff; tải lại và kiểm tra ghi chú còn nguyên.
8. Kiểm tra giao diện desktop/mobile và báo cáo thống kê với dữ liệu SQL Server thật.

Các kết quả chạy thực tế trong môi trường thử được ghi riêng ở docs/online-test-results.json và docs/KIEM_THU_ONLINE.md. Kiểm thử SQLite không thay thế kiểm tra SQL Server và script SSMS trên máy nhóm.
