# Đối chiếu 20 subtask có sẵn

Không tạo lại các subtask này. Mã được bổ sung chưa đồng nghĩa nhiệm vụ Done.

| Jira | Tên subtask | Phần thực hiện | Minh chứng cần lấy |
|---|---|---|---|
| QLBH-145 | API lấy danh sách danh mục | GET /api/categories | JSON danh mục và SELECT Categories |
| QLBH-146 | API tạo/sửa/xóa danh mục | POST/PUT/DELETE /api/categories | Tạo/sửa/xóa danh mục rỗng; xóa danh mục có món phải 409 |
| QLBH-147 | API lấy danh sách món | GET /api/products | JSON trang dữ liệu; đối chiếu ProductID, giá, tên |
| QLBH-148 | API tạo/sửa/xóa món | POST/PUT/DELETE /api/products | CRUD thử; DELETE ngừng bán; dữ liệu âm bị chặn |
| QLBH-149 | API tìm kiếm/lọc món | GET /api/products?search=...&categoryId=...&available=true | Ảnh tham số và kết quả đúng/sai/trống; ghi giới hạn tìm không dấu |
| QLBH-151 | API lấy trạng thái bàn | GET /api/tables | Đối chiếu Tables |
| QLBH-152 | API cập nhật trạng thái bàn | PUT /api/tables/{id}/status | Admin cập nhật hợp lệ; trạng thái trái đơn đang mở 409 |
| QLBH-153 | API tạo đơn | POST /api/orders hoặc /Pos/Add | 201 khi tạo đơn riêng; thêm món đầu tiên tạo đơn; 409 khi tạo trùng bàn đang mở |
| QLBH-154 | API thêm/sửa/xóa chi tiết đơn | /Pos/Add; /Pos/Quantity; PUT /api/orders/{orderId}/items/{detailId} | Thêm, đổi tùy chọn/ghi chú, quantity=0 xóa; đối chiếu OrderDetails |
| QLBH-155 | API tính tổng tiền | GET /Pos/Cart | Công thức giá+size+topping nhân số lượng khớp total |
| QLBH-157 | API tạo thanh toán | POST /Pos/Pay | Đủ tiền thành công; thiếu tiền 400; lặp/sai tổng 409; lưu Orders |
| QLBH-158 | API hoàn tất đơn | POST /Pos/Pay | Đơn Paid, bàn Trống; sửa đơn đóng bị chặn |
| QLBH-159 | API CRUD khách hàng | /api/customers | Thêm/sửa/đọc/ngừng sử dụng; số điện thoại trùng 409 |
| QLBH-160 | API tra cứu lịch sử mua | PUT /api/orders/{orderId}/customer; GET /api/customers/{id}/orders | Gắn trước trả tiền; lịch sử chỉ khách đúng và đơn đã thanh toán |
| QLBH-161 | Viết tài liệu API | docs/API.md và /swagger/v1/swagger.json | Danh sách API, dữ liệu mẫu, lỗi/quyền, link commit và review |
| QLBH-223 | Xác định chỉ số dashboard | docs/DASHBOARD_METRICS.md | Nhóm duyệt định nghĩa, thời gian và nguồn số liệu |
| QLBH-224 | API thống kê doanh thu | GET /api/statistics/revenue | JSON khớp script 06 cùng khoảng ngày |
| QLBH-225 | API thống kê số đơn | GET /api/statistics/orders | Tổng/paid/open/cancelled khớp SQL |
| QLBH-226 | API món bán chạy | GET /api/statistics/best-sellers | SUM Quantity, thứ tự và top khớp SQL |
| QLBH-227 | Hiển thị biểu đồ/thẻ thống kê | /ApiDashboard | Ảnh hai khoảng ngày, video lọc, trạng thái rỗng, khớp API |

Mỗi minh chứng ghi người chạy thực tế, ngày, commit, input, expected/actual, status code, kết luận, người review. Đính kèm response đã bỏ cookie/token; không chụp chỉ danh sách Swagger để thay kết quả chạy.
