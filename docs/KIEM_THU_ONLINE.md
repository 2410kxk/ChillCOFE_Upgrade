# Kiểm thử bản online 20/10/2026

- Build ứng dụng và TestHost: thành công, 0 lỗi, 0 cảnh báo (.NET SDK 8.0.426).
- 44 kiểm tra đạt, 0 kiểm tra thất bại, 1 nhóm kiểm tra trình duyệt bị chặn.
- CSDL thử SQLite riêng, tài khoản/dữ liệu giả lập; không truy cập database của nhóm.
- Đã kiểm tra HTTP/render Razor, phân quyền/CSRF, tùy chọn món, tính giá máy chủ, món hết/mở lại, giỏ tách tài khoản, checkout lặp, quyền sở hữu lịch sử, chuyển trạng thái giao hàng, xác nhận tiền, ghi chú bàn và đăng ký User.
- Kiểm tra trực tiếp dữ liệu: 1 đơn duy nhất, 104.000đ, IsPaid=true; PaidAt và DeliveredAt có giá trị. Lặp xác nhận không thêm đơn hoặc doanh thu.
- JavaScript được kiểm tra cú pháp bằng node --check.
- **Chưa kiểm tra trực quan desktop/mobile:** Chromium chưa có và endpoint tải trả Site Unavailable. Không coi việc Razor render HTTP 200 là bằng chứng giao diện đẹp/không tràn.
- **Chưa chạy script SQL Server hoặc kiểm thử với SQL Server:** cần thực hiện checklist trong README_ONLINE_20261020.md trên máy nhóm. API báo cáo dùng EF/SQL Server cần xác nhận với dữ liệu thật.
- Chi tiết từng kiểm tra: online-test-results.json. Các báo cáo build/visual cũ trong gói là lịch sử bản trước, không phải chứng cứ bản này.
