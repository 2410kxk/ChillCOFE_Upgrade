> Cập nhật 20/10/2026: hướng dẫn ưu tiên là README_ONLINE_20261020.md. Các mô tả ngừng đặt online trong tài liệu cũ không còn áp dụng cho bản này.

# Chill COFÉ — bản nâng cấp ngày 14/10/2026

Được chỉnh sửa từ src.zip và đối chiếu cấu trúc sqlnew.sql bạn gửi. Đây là mã nguồn ASP.NET Core MVC .NET 8, dùng SQL Server/LocalDB. Không cần chuyển sang công nghệ khác.

## 1. Nội dung mới

- Trang chủ nền cà phê toàn khung; dải ảnh chuyển động dưới nav; logo giữa; nav màu nâu; menu đồ uống xổ xuống khi rê chuột hoặc bấm; footer lấy thông tin từ database.
- Font Be Vietnam Pro và Playfair Display có tệp cục bộ; nội dung tiếng Việt không phụ thuộc tải Google Fonts khi mở web.
- Thẻ món có ảnh, tên, giá và liên kết chi tiết; không đưa đoạn mô tả dài lên thẻ. Trang Câu chuyện chuyển về trang chủ.
- Đánh giá 1–5 sao, một ảnh JPG/PNG/WebP tối đa 5 MB, bình luận, sửa/xóa đánh giá của chính mình. Admin có thể xóa đánh giá/bình luận của người khác. Nội dung được Razor mã hóa khi hiển thị.
- Badge Staff và Manager bên tên người viết; khách hàng chỉ có tên. **Manager là nhãn hiển thị của vai trò Admin hiện có**, không tạo vai trò mới.
- Hồ sơ: tên hiển thị, số điện thoại, ảnh đại diện, đổi mật khẩu có kiểm tra mật khẩu hiện tại. Email hiện ở chế độ chỉ đọc. Đăng nhập bằng email được đối chiếu tài khoản, kể cả khi username khác email.
- Chuông thông báo; Admin đăng/gỡ thông báo cho khách hàng, nhân viên hoặc cả hai; mỗi người có trạng thái đã đọc riêng. Cập nhật khi tải trang; chưa triển khai thông báo đẩy hoặc email.
- Không gian làm việc mở trực tiếp /Admin; thêm quản lý đánh giá, thông báo, thông tin quán; thống nhất màu sắc, bảng và biểu mẫu.
- Dashboard /ApiDashboard có đường doanh thu theo ngày, thanh ngang món bán chạy, vòng tròn cơ cấu đơn; số liệu lấy từ API. Không có số liệu thì hiển thị trạng thái rỗng. Doanh thu hiện lọc theo **ngày tạo đơn OrderTime**, chỉ tính đơn đã thanh toán.
- Ghi chú bàn (ví dụ: “2 người · gần cửa sổ”), chú giải Trống/Đang dùng. Ghi chú hiển thị tại POS và API lấy bàn.
- Script SQL bổ sung bảng/cột, giữ tài khoản, đơn hàng và dữ liệu cũ.

## 2. Cài đặt theo đúng thứ tự

### Bước 1 — Sao lưu

1. Dừng web bằng Ctrl+C trong terminal đang chạy.
2. Sao chép thư mục dự án hiện tại thành một bản dự phòng. Không xóa ChillCOFE_Upgrade cũ.
3. Trong SSMS: chuột phải **ChillCOFE_Dev → Tasks → Back Up… → Full → OK**. Giữ file .bak ở nơi bạn nhớ.
4. Giải nén gói mới vào `D:\QLDA CNTT\ChillCOFE_Community_Upgrade`. Thư mục này phải chứa `src`, `database`, `docs` và file hướng dẫn này.
5. Nếu bản cũ có ảnh người dùng tải lên sau thời điểm gửi src.zip, sao chép thêm các ảnh ấy vào đúng thư mục tương ứng trong `src\wwwroot` của bản mới.

### Bước 2 — Nâng cấp SQL trước khi chạy web

1. Mở SSMS, kết nối `(localdb)\MSSQLLocalDB`.
2. Mở file `database\Upgrade_20261014.sql` trong gói mới.
3. Kiểm tra database đang dùng đúng là **ChillCOFE_Dev**, rồi Execute.
4. Kết quả phải có thông báo “Nâng cấp schema hoàn tất” và danh sách bảng.

Script dùng `USE [ChillCOFE_Dev]`. Nếu database thật của bạn mang tên khác, sửa dòng USE cho đúng trước khi Execute. Script dành cho database đã có cấu trúc gốc từ sqlnew.sql, không phải script tạo database trắng. Có thể chạy lại trên database đã nâng cấp: các bảng/cột được kiểm tra trước khi thêm.

**Không chạy lại toàn bộ sqlnew.sql trên database đang sử dụng. Không chạy `dotnet ef database update` thay cho bước này:** bộ migrations cũ chưa được tái lập baseline theo SQL bạn gửi. Gói này đồng bộ model với schema bằng script nâng cấp riêng. Muốn triển khai theo EF migrations ở đợt sau cần thống nhất lịch sử migrations và snapshot với schema thực tế trước.

### Bước 3 — Mở VS Code và kết nối database

Mở folder `ChillCOFE_Community_Upgrade` trong VS Code. Mở Terminal → New Terminal (PowerShell), chạy từng dòng:

```powershell
cd "D:\QLDA CNTT\ChillCOFE_Community_Upgrade"
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=ChillCOFE_Dev;Trusted_Connection=True;TrustServerCertificate=True;" --project ".\src\QuanLiQuanCaffee.csproj"
dotnet user-secrets set "Seed:Enabled" "false" --project ".\src\QuanLiQuanCaffee.csproj"
dotnet restore ".\src\QuanLiQuanCaffee.csproj"
dotnet build ".\src\QuanLiQuanCaffee.csproj"
dotnet dev-certs https --trust
dotnet run --project ".\src\QuanLiQuanCaffee.csproj" --no-launch-profile --urls "https://localhost:7043"
```

Dự án đã có UserSecretsId, không cần `user-secrets init`. Mở đúng URL `https://localhost:7043`. Giữ terminal đang chạy. Nếu báo cổng đã được sử dụng, dừng bản web cũ trước. Không thêm dấu `\` trước dấu `:` trong URL.

### Bước 4 — Kiểm tra giao diện và dữ liệu

1. Ctrl+F5 trên trình duyệt để tải lại CSS/JS.
2. Đăng nhập tài khoản Admin hiện có → Không gian làm việc → Thông tin quán. Nhập địa chỉ, điện thoại, email và giờ mở cửa thật để footer đầy đủ.
3. Kiểm tra menu, ảnh món, danh sách bàn và đơn cũ còn đúng.
4. Tạo một đánh giá và bình luận bằng User, rồi phản hồi bằng Staff và Admin để kiểm tra badge.
5. Admin gửi một thông báo cho User, một cho Staff, một cho tất cả; đăng nhập từng tài khoản để đối chiếu.
6. Vào Tài khoản để đổi tên, điện thoại, ảnh và thử đổi mật khẩu.
7. Quản lý bàn → nhập ghi chú → mở POS kiểm tra.
8. Dashboard → chọn khoảng có đơn đã thanh toán để kiểm tra đủ biểu đồ. Ngày kết thúc trên giao diện được tính đến hết ngày; API nhận `to` là mốc không bao gồm.
9. Tạo một đơn thử, thêm món, thay đổi số lượng và thanh toán. Ghi rõ đây là đơn kiểm thử khi chụp minh chứng.

## 3. Đường dẫn và quyền

| Trang | URL | Quyền |
|---|---|---|
| Trang chủ / menu | `/`, `/Cafe/Menu` | Công khai |
| Chi tiết món | `/Product/Details/{id}` | Công khai |
| Xem đánh giá | `/Reviews` | Công khai |
| Gửi đánh giá, bình luận | Biểu mẫu tại `/Reviews` | Đã đăng nhập |
| Quản lý đánh giá | `/Reviews/Manage` | Admin |
| Hồ sơ / mật khẩu | `/Profile`, `/Profile/Password` | Tài khoản của chính mình |
| Hộp thông báo | `/Notifications` | Đã đăng nhập; theo nhóm nhận |
| Quản lý thông báo | `/Notifications/Manage` | Admin |
| Thông tin quán | `/StoreSettings` | Admin |
| Không gian làm việc | `/Admin` | Admin |
| Bán hàng | `/Pos/Tables` | Admin, Staff |
| Dashboard | `/ApiDashboard` | Admin |
| Swagger | `/swagger` | Có trong Development; từng API kiểm tra quyền riêng |

Chức năng mới đánh giá/hồ sơ/thông báo dùng các action MVC và form có token chống giả mạo, không tự động xuất hiện như API REST trong Swagger. Kiểm thử chúng qua giao diện; không coi danh sách Swagger là toàn bộ chức năng web.

## 4. Thay đổi database

| Thành phần | Thay đổi |
|---|---|
| Reviews, ReviewComments | Tái sử dụng bảng trong SQL đã gửi; chỉ tạo khi chưa tồn tại |
| Tables | Dùng cột Note, thêm nếu thiếu |
| UserProfiles | Bảng mới: tên hiển thị và ảnh đại diện; liên kết AspNetUsers |
| CafeNotifications | Bảng mới: tiêu đề, nội dung, nhóm nhận, tác giả, thời điểm |
| NotificationReads | Bảng mới: khóa ghép thông báo/người đọc, thời điểm đọc |
| StoreSettings | Tái sử dụng cấu hình hiện có, thêm Email và OpeningHours |

Không lưu mật khẩu tài khoản trong các bảng mới; Identity vẫn quản lý mật khẩu. Ảnh mới lưu trong `src/wwwroot/uploads/community`; cần sao lưu cả thư mục ảnh cùng database.

## 5. Minh chứng và kiểm thử

Xem `docs/KIEM_THU_VA_MINH_CHUNG.md`, `docs/integration-results.json` và log build. Kết quả có sẵn là kiểm tra do Codex thực hiện trên môi trường thử độc lập, không phải bằng chứng nhóm đã triển khai trên máy Windows hay dữ liệu thật.

## 6. Khi gặp lỗi

- **ConnectionString chưa cấu hình**: chạy lại Bước 3, chú ý biến Development phải được đặt trong chính terminal chạy web.
- **Invalid object name UserProfiles/CafeNotifications…**: chưa chạy script nâng cấp hoặc app đang nối database khác.
- **Invalid column name Email/OpeningHours/Note**: kiểm tra script SQL đã chạy thành công trên database app sử dụng.
- **Cookie/session cũ, đăng nhập lỗi sau đổi bản**: thử cửa sổ ẩn danh hoặc xóa cookie localhost, đăng nhập lại. Không xóa database.
- **Ảnh 404**: kiểm tra ImageUrl trong Products và tệp tương ứng trong wwwroot/images; ảnh cũ không được script tự đổi đường dẫn.
- **Không thấy quyền Manager**: tên quyền kỹ thuật vẫn là Admin; nhãn Manager chỉ hiện ở đánh giá/bình luận.

## 7. Đưa vào repo

Tạo nhánh nâng cấp, chép phần src/database/docs vào repo đang dùng rồi xem `git diff` trước khi commit. Giữ nguyên thư mục `.git` và tài liệu QLDA hiện có. Không commit secrets, bin/obj, bản .bak hoặc dữ liệu khách hàng thật. Không chép thư mục kiểm thử vào src.

Chỉ đánh dấu Jira Done sau khi nhóm review và chạy lại luồng liên quan trên SQL Server thật.
