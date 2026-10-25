# Tài liệu API Chill COFÉ

Base URL: https://localhost:7043. Cookie Identity; đăng nhập ở website. POST/PUT/DELETE cần RequestVerificationToken cùng phiên. Swagger tự lấy token ở /dev/antiforgery trong Development. Không có JWT/login API mới.

## Quy tắc chung

/api/... chưa đăng nhập: 401; sai quyền: 403. Input lỗi: 400; ID không tồn tại: 404; xung đột nghiệp vụ: 409. API POS dùng route cũ có thể trả redirect khi chưa đăng nhập. Cookie/CSRF không phải tài liệu công khai.

PUT là cập nhật toàn bộ DTO, không phải PATCH; gửi đầy đủ các trường để không vô tình đổi giá trị mặc định. Swagger Schemas cho biết kiểu/range/required, nhưng giá trị mẫu tự sinh không luôn hợp lệ (size phải M/L, topping đúng danh sách).

## Danh sách endpoint theo controller

### CategoriesApiController

- `[HttpGet]`
- `[HttpGet("{id:int}")]`
- `[HttpPost, Authorize(Roles = "Admin")]`
- `[HttpPut("{id:int}"), Authorize(Roles = "Admin")]`
- `[HttpDelete("{id:int}"), Authorize(Roles = "Admin")]`
### CustomersApiController

- `[HttpGet]`
- `[HttpGet("{id:int}")]`
- `[HttpPost]`
- `[HttpPut("{id:int}")]`
- `[HttpDelete("{id:int}"),Authorize(Roles="Admin")]`
- `[HttpGet("{id:int}/orders")]`
- `[HttpPut("/api/orders/{orderId:int}/customer")]`
### PosController

- `[HttpGet]`
- `[HttpGet]`
- `[HttpGet("/Pos/Cart")]`
- `[HttpPost("/Pos/Add")]`
- `[HttpPost("/Pos/Quantity")]`
- `[HttpPost("/Pos/Pay")]`
- `[HttpPost("/api/orders")]`
- `[HttpPost("/api/orders/{id:int}/cancel")]`
- `[HttpPut("/api/orders/{orderId:int}/items/{detailId:int}")]`
- `[HttpGet]`
### ProductsApiController

- `[HttpGet]`
- `[HttpGet("{id:int}")]`
- `[HttpPost, Authorize(Roles="Admin")]`
- `[HttpPut("{id:int}"), Authorize(Roles="Admin")]`
- `[HttpDelete("{id:int}"), Authorize(Roles="Admin")]`
### StatisticsApiController

- `[HttpGet("revenue")]`
- `[HttpGet("orders")]`
- `[HttpGet("best-sellers")]`
### TablesApiController

- `[HttpGet]`
- `[HttpGet("{id:int}")]`
- `[HttpPut("{id:int}/status"), Authorize(Roles="Admin")]`

Các route tương đối ghép với [Route] của controller: Categories=/api/categories, Products=/api/products, Tables=/api/tables, Customers=/api/customers, Statistics=/api/statistics. POS có các route HTML ẩn khỏi Swagger.

## Đăng nhập và kiểm thử trên Swagger

1. Chạy ứng dụng trong môi trường Development.
2. Mở https://localhost:7043 và đăng nhập tài khoản Admin
   hoặc Staff.
3. Mở https://localhost:7043/swagger trong cùng trình duyệt.
4. Chọn endpoint, nhấn Try it out, nhập dữ liệu và Execute.

Ứng dụng sử dụng cookie đăng nhập của website.
Quyền gọi API phụ thuộc vào vai trò và từng endpoint.
Nhóm API thống kê yêu cầu quyền Admin.

## Token chống giả mạo

Các thao tác ghi dữ liệu có kiểm tra antiforgery cần cả
cookie đăng nhập và request token.

Trong cấu hình Swagger của dự án, mã JavaScript lấy token
qua GET /dev/antiforgery và gửi token bằng header:

RequestVerificationToken

Endpoint lấy token chỉ được mở trong môi trường Development,
yêu cầu tài khoản Admin hoặc Staff.

Không ghi mật khẩu, cookie đăng nhập hoặc giá trị token thật
vào tài liệu hay repository.

## Lịch sử mua hàng

- Gắn khách với đơn trước khi thanh toán bằng:
  PUT /api/orders/{orderId}/customer
- Tra cứu bằng:
  GET /api/customers/{id}/orders
- Lịch sử chỉ trả các đơn có IsPaid = true và trạng thái
  "Đã thanh toán".
- Hỗ trợ phân trang bằng page và pageSize.

## Khoảng thời gian thống kê

API hiện lọc theo OrderTime, tức thời điểm tạo đơn.

Điều kiện:
from <= OrderTime < to

Ví dụ thống kê toàn bộ ngày 05/10/2026:
from = 2026-10-05T00:00:00
to   = 2026-10-06T00:00:00

## Input mẫu

Danh mục: {"name":"Danh mục thử"}.

Món: {"name":"Món thử","price":25000,"categoryId":1,"description":"Demo","imageUrl":null,"isAvailable":true,"supportsCustomization":true}. Thay categoryId thực tế.

Khách: {"name":"Khách A","phone":"0900000001","email":null,"isActive":true}.

Tạo đơn: {"tableId":1}. Liên kết khách: {"customerId":1}. Trạng thái bàn: {"status":"Trống"}.

Sửa dòng: {"quantity":2,"size":"L","sugar":50,"ice":50,"topping":"Trân châu","note":"Ít ngọt"}.

Thanh toán: {"tableId":1,"orderId":1,"method":"Tiền mặt","customerPaid":100000,"expectedTotal":74000,"confirmed":true}. Thay ID/tổng đúng từ /Pos/Cart.

Tùy chọn: size M/L; đường 0/30/50/70/100; đá 0/50/100; topping Không/Trân châu/Thạch trái cây/Kem cheese. Món không hỗ trợ tùy chỉnh dùng mặc định M/100/100/Không.

GET thống kê: from=2026-10-04T00:00:00, to=2026-10-05T00:00:00; từ bao gồm, đến không bao gồm. Top từ 1 đến 50. Danh sách món/khách/lịch sử page>=1, pageSize 1..100. Lịch sử chỉ đơn đã thanh toán.

Xem HUONG_DAN.md để thực hiện chuỗi test và hiểu các giới hạn nghiệp vụ. Các mã lỗi dự kiến không thay thế kết quả test.
