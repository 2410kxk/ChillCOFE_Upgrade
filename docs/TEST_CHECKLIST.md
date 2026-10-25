# Checklist kiểm thử — chưa chạy

Dùng DB demo; Admin/Staff/User và phiên chưa đăng nhập. Đổi ID theo dữ liệu thực tế. Ca bị chặn bởi lỗi thiết lập ghi Blocked, không ghi Pass.

| Mã | Tình huống | Mong đợi | Thực tế / trạng thái | Người test / minh chứng |
|---|---|---|---|---|
| API-01 | Admin GET danh mục | 200 và khớp DB | Chưa chạy | |
| API-02 | Staff GET món | 200 | Chưa chạy | |
| API-03 | Chưa đăng nhập GET /api/products | 401, không trả dữ liệu | Chưa chạy | |
| API-04 | User GET /api/products | 403 | Chưa chạy | |
| API-05 | Staff POST danh mục có CSRF hợp lệ | 403 | Chưa chạy | |
| API-06 | Admin POST thiếu CSRF | 400, DB không thay đổi | Chưa chạy | |
| API-07 | Tạo danh mục tên hợp lệ | 201 và ID mới | Chưa chạy | |
| API-08 | Danh mục tên chỉ khoảng trắng | 400 | Chưa chạy | |
| API-09 | Sửa tên danh mục | 200, DB cập nhật | Chưa chạy | |
| API-10 | Xóa danh mục có món | 409, giữ dữ liệu | Chưa chạy | |
| API-11 | Xóa danh mục rỗng | 204 | Chưa chạy | |
| API-12 | Tạo món có danh mục thật | 201 | Chưa chạy | |
| API-13 | Giá món âm | 400 | Chưa chạy | |
| API-14 | Danh mục không tồn tại khi tạo món | 400 | Chưa chạy | |
| API-15 | Tìm/lọc món kết hợp | Đúng bộ lọc | Chưa chạy | |
| API-16 | Tìm món không có kết quả | 200 count=0 items=[] | Chưa chạy | |
| API-17 | PageSize=101 | 400 | Chưa chạy | |
| API-18 | DELETE món | Ngừng bán; lịch sử còn | Chưa chạy | |
| API-19 | Thêm món đã ngừng bán vào POS | 400 | Chưa chạy | |
| API-20 | GET bàn | 200 khớp DB | Chưa chạy | |
| API-21 | Staff PUT trạng thái bàn | 403 | Chưa chạy | |
| API-22 | Admin giải phóng bàn còn đơn | 409 | Chưa chạy | |
| API-23 | Tạo đơn bàn trống | 201 và bàn Đang dùng | Chưa chạy | |
| API-24 | Tạo đơn thứ hai cùng bàn đang mở | 409 | Chưa chạy | |
| API-25 | Hủy đơn mới rỗng | 200 và bàn Trống | Chưa chạy | |
| API-26 | Thêm món M/L và topping | Giá tính đúng | Chưa chạy | |
| API-27 | Thêm quantity=0 hoặc 100 | 400 | Chưa chạy | |
| API-28 | Sửa tùy chọn/ghi chú dòng | 200 và tính lại theo giá menu | Chưa chạy | |
| API-29 | Đổi số lượng dòng | 200 và tổng đúng | Chưa chạy | |
| API-30 | Xóa dòng cuối bằng quantity=0 | Đơn hủy, bàn Trống | Chưa chạy | |
| API-31 | Tạo khách hợp lệ | 201 | Chưa chạy | |
| API-32 | Số điện thoại khách trùng | 409 | Chưa chạy | |
| API-33 | Email khách sai | 400 | Chưa chạy | |
| API-34 | Gắn khách vào đơn đang mở | 200, FK lưu | Chưa chạy | |
| API-35 | Gắn khách ngừng sử dụng | 400 | Chưa chạy | |
| API-36 | Pay confirmed=false | 400 | Chưa chạy | |
| API-37 | Pay tiền mặt thiếu tiền | 400 | Chưa chạy | |
| API-38 | Pay expectedTotal sai | 409 | Chưa chạy | |
| API-39 | Pay đủ tiền | 200, tiền thừa đúng, bàn Trống | Chưa chạy | |
| API-40 | Pay lần hai cùng đơn | 409, không thu lần hai | Chưa chạy | |
| API-41 | Sửa dòng đơn đã thanh toán | 409 | Chưa chạy | |
| API-42 | Lịch sử khách A/B | Không lẫn khách, chỉ đơn đã trả | Chưa chạy | |
| API-43 | DELETE khách có lịch sử | IsActive=false, lịch sử còn | Chưa chạy | |
| API-44 | Chuyển khoản confirmed=true | Lưu phương thức, tiền thừa 0; xác nhận thủ công | Chưa chạy | |
| API-45 | Doanh thu cùng kỳ SQL | Khớp tổng đơn đủ điều kiện | Chưa chạy | |
| API-46 | Số đơn theo trạng thái | Khớp COUNT SQL | Chưa chạy | |
| API-47 | Món bán chạy | Khớp SUM Quantity và thứ tự | Chưa chạy | |
| API-48 | Khoảng ngày rỗng | 0 và items=[] | Chưa chạy | |
| API-49 | from>=to hoặc hơn 366 ngày | 400 | Chưa chạy | |
| API-50 | Staff truy cập thống kê | 403 | Chưa chạy | |
| API-51 | Dashboard đổi hai khoảng ngày | Thẻ/biểu đồ thay đúng, khớp API | Chưa chạy | |
| API-52 | Dashboard mạng lỗi | Thông báo lỗi, không giữ số cũ như kết quả mới | Chưa chạy | |
| API-53 | Hai phiên cùng thanh toán | Không ghi nhận trả hai lần; ghi cả response thực tế/lỗi tranh chấp nếu có | Chưa chạy | |
