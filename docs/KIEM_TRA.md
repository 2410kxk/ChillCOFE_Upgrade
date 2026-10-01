# Kiểm tra bản nâng cấp

Tất cả ca dưới đây CHƯA CHẠY trên SQL Server. Người kiểm tra ghi kết quả thật, thời điểm, build, ảnh/log và bug nếu có.

| Mã | Tình huống | Kỳ vọng | Kết quả |
|---|---|---|---|
| UI-01 | Build bản mới | dotnet restore và dotnet build không có error; chụp log | Chưa chạy |
| UI-02 | Script chạy lại | 03 rồi 04 chạy lại không thêm món trùng tên; số đơn và dòng đơn cũ giữ nguyên | Chưa chạy |
| UI-03 | Trang chủ/menu | Ảnh tải đúng, không tên Phố, không tràn ngang ở 390px và 1366px | Chưa chạy |
| UI-04 | Tìm kiếm | Gõ ca phe tìm Cà phê; không có kết quả hiển thị thông báo | Chưa chạy |
| UI-05 | Lọc/sắp xếp | Chọn Trà sữa và giá tăng dần; chỉ đúng nhóm và thứ tự | Chưa chạy |
| UI-06 | Đơn chuẩn | 2 cà phê sữa + 1 cà phê đen = 70.000đ nếu giữ giá demo | Chưa chạy |
| UI-07 | Tùy chọn | 1 cà phê sữa L + trân châu = 37.000đ; M không topping = 25.000đ | Chưa chạy |
| UI-08 | Gộp dòng | Cùng món/cùng tùy chọn gộp lượng; khác size/đường/đá/topping/ghi chú tách dòng | Chưa chạy |
| UI-09 | Món tiêu chuẩn | Tiramisu không chọn size/topping; server vẫn thu đúng 49.000đ | Chưa chạy |
| UI-10 | Giá giả mạo | Gửi giá giả trong request Add; server bỏ qua và tính bằng giá DB | Chưa chạy |
| UI-11 | Số lượng | 0/âm/100 khi Add bị chặn; thay Quantity=0 xóa dòng đúng đơn | Chưa chạy |
| UI-12 | Xóa hết món | Đơn chuyển Đã hủy, bàn Trống; đơn rỗng không thanh toán | Chưa chạy |
| UI-13 | Món tạm hết | Admin tắt món; POS không thêm được kể cả client giữ màn hình cũ | Chưa chạy |
| UI-14 | Thiếu tiền | Đơn 70.000đ, nhập 50.000đ: không thanh toán, dữ liệu không đổi | Chưa chạy |
| UI-15 | Tiền thừa | Đơn 70.000đ, nhập 100.000đ: CustomerPaid=100000, ChangeAmount=30000 | Chưa chạy |
| UI-16 | Chuyển khoản | Bắt buộc checkbox xác nhận; CustomerPaid bằng tổng, ChangeAmount=0 | Chưa chạy |
| UI-17 | Đơn thay đổi | Hai phiên: phiên 2 thêm món sau khi phiên 1 mở thanh toán; phiên 1 bị yêu cầu kiểm tra lại | Chưa chạy |
| UI-18 | Gửi thanh toán lặp | Cùng OrderId thanh toán lần 2 bị từ chối; không trả nhầm đơn mới cùng bàn | Chưa chạy |
| UI-19 | Sửa bàn khác | Quantity với OrderId/DetailId của bàn khác bị từ chối | Chưa chạy |
| UI-20 | Phân quyền | User/khách không gọi được Pos; Staff không đổi trạng thái món qua CatalogAdmin | Chưa chạy |
| UI-21 | CSRF | Thiếu token khi Add/Quantity/Pay bị từ chối | Chưa chạy |
| UI-22 | In phiếu | Phiếu đúng món, size, topping, tổng, tiền thừa; print không in header/footer web | Chưa chạy |
| UI-23 | Doanh thu | Sau thanh toán tổng doanh thu tăng đúng tiền; đơn mở/hủy không được cộng | Chưa chạy |
| UI-24 | Quản lý món | Thêm/sửa với giá âm/file quá 5MB bị chặn; hợp lệ lưu được; ảnh cũ giữ nếu không tải mới | Chưa chạy |
| UI-25 | Concurrent checkout | Hai phiên gửi Pay đồng thời: chỉ một lần thành công; lỗi tranh chấp phải hiển thị và kiểm tra trạng thái trước thử lại | Chưa chạy |
| UI-26 | Khởi động lại | Dừng/chạy app; đơn đã trả, bàn, dữ liệu menu vẫn đúng | Chưa chạy |
