# Kiểm thử và lấy minh chứng — Chill COFÉ

## Kết quả thực hiện bởi Codex

Ngày 14/10/2026. Build .NET 8 thành công, không còn cảnh báo trong lần build cuối. Môi trường chức năng: test host độc lập, EF Core SQLite, tài khoản và dữ liệu giả lập. Không kết nối hoặc thay đổi SQL Server của người dùng.

**33/33 kiểm tra đạt.** Kiểm tra render xác nhận HTTP 200 và Razor thực thi; không thay thế toàn bộ nghiệm thu UI.

| STT | Kiểm tra | Kết quả |
|---|---|---|
| 1 | Trang chủ công khai | Đạt |
| 2 | Khách chưa đăng nhập không vào hồ sơ | Đạt |
| 3 | Đăng nhập alice | Đạt |
| 4 | Đăng nhập bob | Đạt |
| 5 | Đăng nhập staff | Đạt |
| 6 | Đăng nhập admin | Đạt |
| 7 | Render /Admin | Đạt |
| 8 | Render /Reviews/Manage | Đạt |
| 9 | Render /Notifications/Manage | Đạt |
| 10 | Render /StoreSettings | Đạt |
| 11 | Render /ApiDashboard | Đạt |
| 12 | Render /Table | Đạt |
| 13 | Render /Pos/Tables | Đạt |
| 14 | Render /Cafe/Menu | Đạt |
| 15 | Render /Profile | Đạt |
| 16 | Render /Profile/Password | Đạt |
| 17 | Chặn ghi thiếu CSRF | Đạt |
| 18 | Chặn sao ngoài 1–5 | Đạt |
| 19 | Tạo đánh giá và tải ảnh | Đạt |
| 20 | Không xóa đánh giá người khác | Đạt |
| 21 | Không sửa đánh giá người khác | Đạt |
| 22 | Bình luận hiện nhãn Staff/Manager | Đạt |
| 23 | Staff không tạo thông báo quản lý | Đạt |
| 24 | Thông báo đúng nhóm User | Đạt |
| 25 | Thông báo đúng nhóm Staff | Đạt |
| 26 | Không đánh dấu thông báo ngoài nhóm | Đạt |
| 27 | Đánh dấu đã đọc lặp lại an toàn | Đạt |
| 28 | Lưu tên và điện thoại | Đạt |
| 29 | Không đổi mật khẩu khi mật khẩu cũ sai | Đạt |
| 30 | Đổi mật khẩu đúng | Đạt |
| 31 | Ghi chú bàn hiện ở POS | Đạt |
| 32 | Manager xóa đánh giá và bình luận | Đạt |
| 33 | API thống kê từ chối khoảng ngày sai | Đạt |

## Phần nhóm cần kiểm tra trên máy thật

Các mục sau CHƯA được xác nhận trên SQL Server của nhóm; không đánh dấu Pass trước khi thực hiện.

| Mục | Thao tác | Minh chứng cần lưu |
|---|---|---|
| SQL nâng cấp | Backup, chạy Upgrade_20261014.sql; chạy lần hai để kiểm tra không tạo trùng | Ảnh DB đang chọn và kết quả script, không hiện dữ liệu nhạy cảm |
| Dữ liệu cũ | Đối chiếu số món, bàn, đơn và tài khoản trước/sau | Truy vấn COUNT và ảnh màn hình |
| Giao diện | Mở desktop và chế độ điện thoại; hover/click menu, mở chi tiết món | Ảnh trang chủ, menu và nav |
| Đánh giá | User gửi 1–5 sao và ảnh, Staff phản hồi, Admin xóa | Ảnh trước/sau, badge và bản ghi SQL tương ứng |
| Ảnh không hợp lệ | Tải .txt hoặc ảnh lớn hơn 5 MB | Thông báo từ chối và xác nhận không lưu bản ghi mới |
| Hồ sơ | Đổi tên/điện thoại/ảnh rồi tải lại trang | Ảnh trước/sau; không chụp mật khẩu |
| Mật khẩu | Sai mật khẩu cũ bị chặn; đúng thì đổi; đăng nhập bằng mật khẩu mới | Kết quả thực tế, che dữ liệu đăng nhập |
| Thông báo | Gửi riêng User/Staff/All; người ngoài nhóm không thấy | Ảnh từ hai tài khoản và trạng thái đọc |
| Bàn | Lưu ghi chú, xem ở POS và GET /api/tables | Ảnh UI + JSON có note |
| Biểu đồ | Chọn thời gian có đơn đã thanh toán; so với SQL | Ảnh bar/line/donut, bộ lọc, truy vấn doanh thu |
| POS hồi quy | Tạo đơn → thêm món → sửa số lượng → thanh toán → bàn trống | Video ngắn hoặc ảnh từng bước; ID đơn thử |
| Cookie/CSRF | Đăng xuất rồi thử truy cập quản lý; thao tác ghi thiếu token | Mã phản hồi/ảnh chuyển đăng nhập |

## Đính kèm Jira

1. Mở subtask có phạm vi tương ứng (không tạo task trùng chỉ vì cách viết tên khác).
2. Đính kèm ảnh và ghi URL màn hình, tài khoản theo vai trò, ID dữ liệu thử, kết quả mong đợi/thực tế.
3. Thêm link commit/PR chứa thay đổi. Không dùng riêng ảnh giao diện để chứng minh DB hoặc phân quyền.
4. Ghi người thực hiện, người review và ngày thực hiện thật. Bản kiểm tra của Codex là tài liệu hỗ trợ, không thay thế minh chứng cá nhân.

## Giới hạn đã biết

- Chưa chạy script T-SQL trên SQL Server/LocalDB trong môi trường này; cần xác nhận backup, schema và dữ liệu thật tại máy nhóm.
- Chưa kiểm thử toàn bộ hồi quy các chức năng cũ (ca làm, báo cáo, import/export, thanh toán đồng thời).
- Migrations cũ được giữ để tham khảo; đợt nâng cấp này dùng SQL thủ công, không tự cập nhật lịch sử EF migrations.
- Thông báo là thông báo trong website, cần tải lại trang; ảnh review tối đa một ảnh mỗi đánh giá.
- Chưa tích hợp thanh toán ngân hàng trực tuyến hay gửi email; không dùng bản này làm căn cứ tuyên bố các chức năng đó đã hoàn thành.

## Kiểm tra giao diện

Đã mở giao diện bằng Chromium ở 1440×1000 và 390×844; kiểm tra trang chủ, thẻ món, font tiếng Việt, trang quản lý, hồ sơ và đánh giá. Không có lỗi JavaScript được ghi nhận trong lượt xem. Biểu đồ được kiểm tra bố cục với phản hồi API giả lập; chưa xác nhận tổng hợp doanh thu thực tế trên SQL Server.
