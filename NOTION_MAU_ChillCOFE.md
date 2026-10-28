# CHILL COFÉ — Không gian làm việc nhóm

> Mẫu nội dung Notion • phiên bản 1.0 • lập ngày 06/10/2026. Đây là mẫu để nhóm đưa vào Notion, chưa phải workspace Notion đã tạo. Ô “Chưa xác nhận” cần được nhóm kiểm tra; không đổi thành Đạt chỉ vì đã có mẫu.

## 1. Tổng quan dự án

| Thuộc tính | Nội dung |
|---|---|
| Tên | Chill COFÉ — Hệ thống bán hàng và thanh toán cho quán cà phê |
| Học phần | Quản lý dự án công nghệ thông tin |
| Thời gian theo lịch học phần | 07/09–01/11/2026, 8 tuần |
| Mốc báo cáo giữa kỳ | Kết quả đến hết 04/10/2026; cập nhật từ 05/10 ghi riêng |
| Quy mô | 5 sinh viên |
| Sản phẩm đang phát triển | Web ASP.NET Core MVC, Identity, EF Core và SQL Server |
| Thay đổi phạm vi cần xác nhận | Đề bài ban đầu yêu cầu Desktop; tình trạng chấp thuận Web chưa xác minh |
| Môi trường demo hiện có theo hội thoại | Local Windows, VS Code, HTTPS localhost:7043 |
| CSDL phát triển | ChillCOFE_Dev trên (localdb)\MSSQLLocalDB |
| Tình trạng trang này | Mẫu chờ nhóm cập nhật và review |

### Liên kết làm việc

- [GitHub của nhóm : https://github.com/nguyenvany9a2-alt/ChillCOFE\_Upgrade](https://github.com/nguyenvany9a2-alt/ChillCOFE_Upgrade)
- [Jira QLBH : https://nguyenvany9a2-1790123995602.atlassian.net/jira/software/projects/QLBH/boards/2/backlog?epics=visible](https://nguyenvany9a2-1790123995602.atlassian.net/jira/software/projects/QLBH/boards/2/backlog?epics=visible)
- [Balsamiq — thiết kế giao diện :](https://balsamiq.cloud/sh2dq3b/poxx5y0) [https://balsamiq.cloud/sh2dq3b/poxx5y0/rA310](https://balsamiq.cloud/sh2dq3b/poxx5y0/rA310)
- Báo cáo giữa kỳ: [Nhóm gắn file hoặc link phiên bản đang review]
- Thư mục minh chứng: [Nhóm điền link có quyền xem]
- Nhật ký AI: `docs/AI_LOG.md` trong repo — [bổ sung permalink sau commit]

Các liên kết trên do người dùng cung cấp, chưa xác minh quyền truy cập của giảng viên. Balsamiq là công cụ đang được dẫn, không ghi là Figma.

## 2. Thành viên và trách nhiệm

Tên lấy từ bìa báo cáo; tài khoản lấy từ CSV trước đã được đối chiếu. Đây là phân công đầu việc quan sát được, không thay thế RACI được nhóm duyệt.

| Thành viên trong báo cáo | Tài khoản Jira đã đối chiếu | Nhóm việc đang được giao | Xác nhận |
|---|---|---|---|
| Nguyễn Văn Ý | Nguyễn Văn Ý | PM, backend và CSDL | Chờ xác nhận vai trò chính/phụ |
| Nguyễn Thành Tài | Nguyễn Thành Tài | Yêu cầu, SRS, backlog | Chờ xác nhận |
| Tạ Trần Trọng Phú | Trọng Phú | UI và kiểm thử nghiệp vụ lõi | Chờ xác nhận tên tài khoản |
| Nông Minh Thuận | Nongminhthuan.16062005 | Nghiệp vụ, tích hợp, Git, triển khai | Chờ xác nhận tên tài khoản |
| Nguyễn Lê Thanh Thuận | Chưa xác nhận | Chưa gắn tự động với tài khoản caublack5 | Nhóm xác nhận ánh xạ |

Tài khoản `caublack5` được giao dashboard, QA và tài liệu trong CSV. Không suy đoán danh tính chỉ dựa vào việc còn một tên chưa ánh xạ.

## 3. Kế hoạch 8 tuần

| Tuần | Thời gian | Đầu ra định hướng | Thực tế / link |
|---|---|---|---|
| 1 | 07–13/09 | Khởi động, khảo sát, Charter và phạm vi | Chưa đối chiếu |
| 2 | 14–20/09 | SRS, WBS, lịch, ngân sách, yêu cầu nghiệm thu | Chưa đối chiếu |
| 3 | 21–27/09 | UML, CSDL, prototype và chuẩn bị phát triển | Chưa đối chiếu |
| 4 | 28/09–04/10 | API/giao diện cốt lõi; tổng hợp giữa kỳ | Chưa đối chiếu đầy đủ |
| 5 | 05–11/10 | Hoàn thiện luồng lõi, quyền và test ban đầu | Điền kế hoạch đã thống nhất |
| 6 | 12–18/10 | Tích hợp, báo cáo, import/export, gợi ý theo phạm vi | Điền kế hoạch đã thống nhất |
| 7 | 19–25/10 | Kiểm thử, sửa lỗi, regression | Điền kế hoạch đã thống nhất |
| 8 | 26/10–01/11 | Demo, nghiệm thu, hồ sơ và vấn đáp | Điền kế hoạch đã thống nhất |

Bảng là trang tổng hợp định hướng, không phải Gantt baseline đã phê duyệt. Gắn bản baseline nguồn và ngày duyệt tại đây: [chưa có xác nhận].

## 4. Bảng công việc và minh chứng

Jira là nguồn quản lý trạng thái chính. Notion dùng để tổng hợp, không tạo thêm trạng thái độc lập dễ mâu thuẫn.

| Mã Jira | Tên | Người thực hiện | Hạn đã chốt | Trạng thái Jira / ngày kiểm tra | Đầu ra | Minh chứng | Review |
|---|---|---|---|---|---|---|---|
| [QLBH-…] | [Tên nguyên gốc] | [Tên thật] | [Ngày hoặc chưa có] | [Trạng thái / ngày] | [File/commit] | [Link + trang/phút] | [Chưa review] |

- [ ] Tên/mã khớp Jira, không tạo lại subtask có sẵn.
- [ ] Minh chứng đúng người, phiên bản, dữ liệu thử và thời điểm.
- [ ] Done chỉ khi tiêu chí đạt và đã review theo quy định nhóm.
- [ ] Nếu có thay đổi phạm vi/hạn, ghi Change Log thay vì xóa dấu vết cũ.

## 5. Status report tuần

### Tuần [số] — kỳ [từ ngày] đến [ngày]

- Ngày lập thực tế: [...]
- Người lập: [...]
- Nguồn dữ liệu và ngày chốt: [...]
- Tình trạng tổng thể: Chưa đánh giá / Đúng kế hoạch / Có rủi ro / Trễ — kèm căn cứ.

| Hạng mục | Kế hoạch | Thực tế | Chênh lệch / nguyên nhân | Hành động | Owner / hạn |
|---|---|---|---|---|---|
| [Hạng mục] | [...] | [...] | [...] | [...] | [...] |

Kết quả trong kỳ: [...]
Blocker cần hỗ trợ: [...]
Quyết định cần xin: [...]
Kế hoạch tuần tiếp theo: [...]
Minh chứng: [...]

Không dùng số issue Done/Tổng issue làm EVM. Khi tính tỷ lệ số đầu việc, chỉ đếm một cấp công việc và nói rõ không phản ánh giờ công.

## 6. Họp nhóm và quyết định

### Biên bản [mã] — [ngày thực tế]

- Mục đích, hình thức, thời gian: [...]
- Người tham dự thật: [...]
- Nội dung đã trao đổi: [...]
- Quyết định được thống nhất: [...]
- Ý kiến còn khác nhau/chưa duyệt: [...]

| Việc tiếp theo | Mã Jira | Người phụ trách | Hạn | Xác nhận |
|---|---|---|---|---|
| [...] | [...] | [...] | [...] | [...] |

Minh chứng buổi họp/phản hồi: [...]. Không dùng mẫu này để ghi một cuộc họp chưa diễn ra.

## 7. Rủi ro, vấn đề và thay đổi

### Rủi ro đề xuất cần nhóm đánh giá

| Mã | Rủi ro | Dấu hiệu | Cách ứng phó đề xuất | Owner / đánh giá |
|---|---|---|---|---|
| RK-01 | Desktop/Web chưa thống nhất | Báo cáo và sản phẩm khác nền tảng | Xin xác nhận phạm vi, cập nhật tài liệu | Chưa phân công/chấm điểm |
| RK-02 | Thiếu kiểm thử giao dịch | Checklist chưa có actual | Chạy ca thiếu tiền, gửi lặp, đồng thời và đối chiếu DB | Chưa phân công/chấm điểm |
| RK-03 | Thiếu minh chứng 5 người | Chưa đủ commit/PR/test run | Tổng hợp đóng góp thực và review | Chưa phân công/chấm điểm |

### Change Log

| Mã CR | Nội dung/lý do | Ảnh hưởng phạm vi–lịch–chi phí–test | Người đề xuất | Quyết định/người duyệt/ngày | Baseline liên quan |
|---|---|---|---|---|---|
| [CR-…] | [...] | [...] | [...] | Đề xuất — chưa duyệt | [...] |

## 8. Kiểm thử và lỗi

Bộ ZIP được kiểm tra ngày 06/10 có `docs/KIEM_TRA.md` với 26 tình huống và `docs/TEST_CHECKLIST.md` với 53 tình huống, đều ghi Chưa chạy. Đây là snapshot tài liệu, không khẳng định nhóm chưa từng test bằng cách khác.

| Test | Jira | Build/commit | Người/ngày chạy | Input và bước | Expected | Actual | Kết quả | Evidence/defect |
|---|---|---|---|---|---|---|---|---|
| [TC-…] | [...] | [...] | [...] | [...] | [...] | Chưa chạy | Not Run | [...] |

Tổng hợp mỗi build: Pass [...], Fail [...], Blocked [...], Not Run [...]. Nêu mẫu số pass rate đã chốt. Không coi vài test Pass là đã đủ kiểm thử phạm vi Must.

## 9. Nguồn lực và chi phí

### Timesheet

| Ngày | Thành viên | Jira | Công việc | Giờ thực tế | Link đầu ra | Xác nhận |
|---|---|---|---|---|---|---|
| [...] | [...] | [...] | [...] | [Chưa ghi] | [...] | [...] |

### Thu chi

| Ngày | Khoản mục | Dự toán | Thực chi | Chứng từ | Người chi / xác nhận |
|---|---|---|---|---|---|
| [...] | [...] | [...] | [Chưa xác minh] | [...] | [...] |

Ngân sách đề xuất đang có trong báo cáo: 40 triệu giờ công quy đổi + 4 triệu chi trực tiếp + 1 triệu dự phòng. Cần thống nhất lại Charter; không ghi đây là số tiền đã chi. EVM dùng phạm vi giờ công phải tách với sổ thu chi tiền mặt.

## 10. Pareto và đánh giá thành viên

### Dữ liệu Pareto

| Mã vấn đề | Mô tả thật | Nhóm nguyên nhân chính | Nguồn | Ngày xác nhận |
|---|---|---|---|---|
| [VĐ-…] | [...] | [...] | [...] | [...] |

Nhóm đếm tần suất, sắp xếp giảm dần, tính tỷ lệ lũy kế và gắn biểu đồ tại đây. Chưa có số thực tế thì không tạo kết quả 80/20 giả.

### Tiêu chí đánh giá đề xuất

| Tiêu chí | Trọng số |
|---|---:|
| Hoàn thành nhiệm vụ | 30% |
| Chất lượng | 25% |
| Đúng tiến độ | 15% |
| Phối hợp | 15% |
| Minh chứng và giải thích công việc | 15% |

Nhóm thống nhất thang 1–5, căn cứ từng điểm và người review trước khi chấm. Gắn phiếu của 5 thành viên và bảng tổng hợp, không tự điền điểm.

## 11. Sử dụng AI

- Tài liệu cấu hình: `PROJECT_AI.md`.
- Nhật ký: `docs/AI_LOG.md`.
- Người chịu trách nhiệm với sản phẩm: thành viên thực hiện và người review, không phải AI.
- AI hỗ trợ soạn/phân tích/mã nguồn; kết quả phải được nhóm kiểm tra.
- [ ] Bổ sung tên thật người sử dụng từng lượt.
- [ ] Liên kết prompt với file/commit và test.
- [ ] Lưu ảnh cấu hình Project; không công khai secrets.

## 12. Danh mục minh chứng nộp báo cáo

| Mã | Tiêu chí | File/link | Vị trí trang/phút | Người/ngày thực hiện | Review |
|---|---|---|---|---|---|
| MC-01 | Charter, phát biểu và phạm vi | [Bổ sung] | [...] | [...] | Chưa xác nhận |
| MC-02 | WBS, Gantt và baseline | [Bổ sung] | [...] | [...] | Chưa xác nhận |
| MC-03 | CPM, đường ngắn nhất, thuật toán | [Bổ sung] | [...] | [...] | Chưa xác nhận |
| MC-04 | Trọng số chọn dự án | [Bổ sung] | [...] | [...] | Chưa xác nhận |
| MC-05 | Status report, chi phí | [Bổ sung] | [...] | [...] | Chưa xác nhận |
| MC-06 | Pareto, đánh giá 5 người | [Bổ sung] | [...] | [...] | Chưa xác nhận |
| MC-07 | Jira ít nhất 50 Task | [CSV đầy đủ + ảnh] | [...] | [...] | Chưa xác nhận |
| MC-08 | GitHub, Notion, Balsamiq | [Link + ảnh/PR] | [...] | [...] | Chưa xác nhận |
| MC-09 | Prompt và Project AI | [AI_LOG + cấu hình] | [...] | [...] | Chưa xác nhận |

## 13. Việc cần làm ngay

- [ ] Xuất Jira toàn bộ; bản gửi 06/10 chỉ có 168 Subtask To Do.
- [ ] Đồng bộ lịch và mốc 4 tuần đầu trong báo cáo.
- [ ] Chốt bảng phân công và ánh xạ tên/tài khoản.
- [ ] Hoàn thiện bài 3, bài 5 và các nguồn số liệu.
- [ ] Chạy test và gắn minh chứng thật.
- [ ] Kiểm tra quyền xem các link bằng tài khoản người nhận dự kiến.
