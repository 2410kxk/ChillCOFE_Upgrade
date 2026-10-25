# Chỉ số dashboard — bản đề xuất cần nhóm review (QLBH-223)

Khoảng lọc API: from <= Orders.OrderTime < to, tối đa 366 ngày. Input không kèm timezone, theo giờ lưu trên máy server. UI nhận ngày kết thúc bao gồm và đổi sang 00:00 ngày kế tiếp.

| Chỉ số | Định nghĩa | Nguồn |
|---|---|---|
| Doanh thu | Tổng TotalAmount của đơn IsPaid=true và Status=Đã thanh toán được TẠO trong kỳ | Orders |
| Đơn đã thanh toán | Số đơn đủ hai điều kiện thanh toán trên | Orders |
| Tổng đơn | Tất cả đơn được tạo trong kỳ | Orders |
| Đơn đang mở | IsPaid=false, Status=Đang xử lý | Orders |
| Đơn hủy | Status=Đã hủy | Orders |
| Món bán chạy | Tổng Quantity theo ProductID từ chi tiết của đơn đã thanh toán trong kỳ, giảm dần; bằng nhau xếp ProductID tăng dần | OrderDetails, Orders, Products |
| Thành tiền món | Tổng Price x Quantity, trước giảm giá cấp đơn; không coi là doanh thu ròng | OrderDetails |

Không trừ DiscountAmount lần nữa vì dùng TotalAmount đã lưu. Cần kiểm tra quy tắc dữ liệu cũ có đúng không. Không đưa đơn hủy/chưa trả tiền vào doanh thu. Bảng trống => 0 và danh sách trống. Chưa có nghiệp vụ hoàn tiền nên không khẳng định đây là doanh thu kế toán sau hoàn trả.
Các API gọi độc lập, khi có giao dịch đồng thời có thể chênh tạm thời giữa thẻ; đối chiếu trên dữ liệu demo ổn định.
Người duyệt: Chưa điền. Ngày duyệt: Chưa điền.
