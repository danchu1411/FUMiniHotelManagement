# UI Requirement Mapping Specification

Tài liệu này mô tả cách ánh xạ yêu cầu nghiệp vụ sang giao diện WPF của hệ thống khách sạn. Mục tiêu là giúp quá trình thiết kế, triển khai và kiểm thử UI bám sát đúng requirement gốc.

## Quy ước sử dụng
- **BẮT BUỘC**: phải có để đáp ứng requirement.
- **KHUYẾN NGHỊ**: nên có để tăng trải nghiệm người dùng, nhưng không làm thay đổi nghiệp vụ.
- **TÙY CHỌN**: cải thiện giao diện hoặc trải nghiệm, không phải yêu cầu chức năng.

## 1. Xác thực và phân quyền

### 1.1. Login Window
**BẮT BUỘC**
- Login Window là màn hình đầu tiên của ứng dụng.
- Người dùng đăng nhập bằng **Email** và **Password**.
- Hệ thống phải kiểm tra 2 loại tài khoản:
  - **Admin**: đối chiếu với thông tin cấu hình trong `appsettings.json`
    - Email: `admin@FUMiniHotelSystem.com`
    - Password: `@@abc123@@`
  - **Customer**: đối chiếu với dữ liệu trong bảng `Customer`.

**Ánh xạ UI theo mẫu đề xuất**
- Cửa sổ có thể dùng kiểu hiện đại như mẫu:
  - `WindowStyle="None"`
  - `Background="Transparent"`
  - `AllowsTransparency="True"`
  - `WindowStartupLocation="CenterScreen"`
- Khung đăng nhập nên dùng `Border` bo góc, có viền gradient và nền gradient tối để tạo điểm nhấn.
- Bố cục màn hình login gồm:
  - Tiêu đề `LOGIN WINDOW` ở phía trên
  - Ô nhập `Email/Username`
  - Ô nhập `Password`
  - Nút `LOG IN`
  - Nút `CANCEL`
- Ô nhập phải có nhãn rõ ràng, màu chữ dễ đọc, và hiển thị trạng thái focus tốt.
- Nút `LOG IN` dùng để thực hiện xác thực.
- Nút `CANCEL` dùng để đóng hoặc thoát màn hình đăng nhập.

**Yêu cầu nghiệp vụ vẫn bắt buộc**
- Đăng nhập thành công sẽ mở `Dashboard`.
- Sau khi xác thực, giao diện phải hiển thị theo đúng vai trò của người dùng.

**TÙY CHỌN về giao diện**
- Có thể dùng `PasswordBox` thay vì `TextBox` cho mật khẩu.
- Có thể đổi màu nút khi hover.
- Có thể dùng font hiện đại như Montserrat nếu hệ thống hỗ trợ.
- Có thể thêm hiệu ứng bo tròn và gradient như trong mẫu.

### 1.2. Phân quyền hiển thị
**BẮT BUỘC**
- Admin được truy cập đầy đủ các chức năng quản trị.
- Customer chỉ được truy cập các chức năng được phép theo requirement.

## 2. Bố cục Dashboard và điều hướng

### 2.1. Thanh điều hướng bên trái
**BẮT BUỘC**
- Dashboard phải có thanh điều hướng để chuyển giữa các phân hệ.
- Các tab chính cần tương ứng với các phân hệ nghiệp vụ:
  - `Room / RoomType`
  - `Customer`
  - `Order`
  - `Stat / Status Report`

**Theo vai trò**
- **Admin**: có quyền truy cập cả 4 tab.
- **Customer**: không hiển thị hoặc disable tab `Stat / Status Report`.

**TÙY CHỌN**
- Nền sidebar dạng gradient.
- Hiển thị tên người dùng đang đăng nhập ở cuối sidebar.
- Có nút `Logout`.
- Có thể dùng icon và hiệu ứng chuyển tab để tăng trải nghiệm.

## 3. Ánh xạ theo từng tab

### 3.1. Tab `Room / RoomType`
**BẮT BUỘC**
- **Admin**:
  - Quản lý `RoomInformation` và `RoomType`.
  - Có đầy đủ CRUD và tìm kiếm.
  - Xóa phòng phải tuân theo quy tắc `Hard Delete / Soft Delete`.
- **Customer**:
  - Tìm phòng trống.
  - Nhập khoảng ngày lưu trú và chọn `RoomType` để lọc phòng khả dụng.
  - Chỉ được xem và đặt phòng, không được sửa dữ liệu hệ thống.

### 3.2. Tab `Customer`
**BẮT BUỘC**
- **Admin**:
  - Quản lý danh sách `Customer`.
  - Có CRUD và tìm kiếm.
- **Customer**:
  - Chỉ được xem và cập nhật hồ sơ cá nhân của chính mình.
  - Được đổi mật khẩu.
  - Không được sửa dữ liệu của người dùng khác.

### 3.3. Tab `Order`
**BẮT BUỘC**
- **Admin**:
  - Quản lý `BookingReservation` và `BookingDetail`.
  - Có các thao tác `Check-in` và `Check-out`.
  - Khi `Check-out`, hệ thống phải mở hóa đơn trước khi cập nhật trạng thái cuối cùng.
- **Customer**:
  - Xem lịch sử đặt phòng cá nhân.
  - Chỉ được hủy đơn khi `BookingStatus = 1`.

**Quy ước trạng thái**
- `BookingStatus = 1`: Booked / Reserved
- `BookingStatus = 2`: Checked-In
- `BookingStatus = 3`: Checked-Out
- `BookingStatus = 0`: Cancelled

### 3.4. Tab `Stat / Status Report`
**BẮT BUỘC**
- Chỉ Admin được truy cập.
- Lọc báo cáo theo `StartDate` và `EndDate`.
- Kết quả phải được sắp xếp giảm dần theo thời gian.
- Có thể hiển thị các chỉ số tổng hợp:
  - Tổng doanh thu
  - Loại phòng thuê nhiều nhất
  - Khách hàng đặt phòng nhiều nhất

## 4. Quy tắc UI chung

### 4.1. Tìm kiếm và lọc dữ liệu
**BẮT BUỘC**
- Các màn hình quản lý phải có chức năng tìm kiếm hoặc lọc dữ liệu.
- **Admin**:
  - Dùng `TextBox` / `ComboBox` để tìm `Customer`, `Room`, `Order` theo tiêu chí.
- **Customer**:
  - Ở màn hình tìm phòng trống, dùng `DatePicker` và `ComboBox RoomType`.

### 4.2. Thêm mới và cập nhật
**BẮT BUỘC**
- Tất cả thao tác `Create` và `Update` phải mở `Popup/Dialog` riêng.
- Không cho phép chỉnh sửa trực tiếp trên `DataGrid`.

### 4.3. Xóa dữ liệu
**BẮT BUỘC**
- Khi xóa phải luôn có hộp thoại xác nhận.
- Với `RoomInformation`:
  - Nếu chưa có lịch sử `BookingDetail` thì cho phép xóa vật lý.
  - Nếu đã từng có lịch sử `BookingDetail` thì chỉ được xóa logic bằng cách đổi `RoomStatus`.

### 4.4. Phân trang
**KHUYẾN NGHỊ**
- Có thể dùng phân trang để cải thiện hiệu năng khi danh sách dài.
- Đây là cải thiện UI, không phải requirement bắt buộc.

## 5. Ánh xạ các quy tắc nghiệp vụ quan trọng

### 5.1. Kiểm tra trùng lịch phòng
**BẮT BUỘC**
- Khi lưu `BookingDetail`, phải kiểm tra trùng lịch theo `RoomID`.
- Nếu khoảng thời gian `StartDate / EndDate` chồng lấn với đơn đặt phòng đang hoạt động khác, hệ thống phải chặn lưu.
- Thông báo lỗi phải hiển thị rõ trên UI.

### 5.2. Tính giá phòng động
**BẮT BUỘC**
- Khi tạo `BookingDetail`, hệ thống phải tự tính `ActualPrice`.
- Nếu `StartDate` là Thứ Bảy hoặc Chủ Nhật:
  - `ActualPrice = RoomPricePerDay * 1.2`
- Nếu là ngày làm việc:
  - `ActualPrice = RoomPricePerDay`

### 5.3. Check-out và hóa đơn
**BẮT BUỘC**
- Khi Admin chọn `Check-out`, UI phải mở màn hình hóa đơn.
- Chỉ khi xác nhận xong hóa đơn mới được cập nhật:
  - `BookingStatus = 3`
  - `RoomStatus = 3`

### 5.4. Hủy đặt phòng
**BẮT BUỘC**
- Customer chỉ được hủy đơn khi `BookingStatus = 1`.
- Khi hủy:
  - đổi `BookingStatus` sang `0`
  - giải phóng các phòng liên quan về `RoomStatus = 1`
- Nếu `BookingStatus = 2` hoặc `3`, nút hủy phải disable hoặc ẩn.

## 6. Quy tắc kiểm tra dữ liệu nhập

**BẮT BUỘC**
- `EmailAddress`:
  - đúng định dạng
  - không được trùng trong database
- `Telephone`:
  - chỉ chứa số
  - độ dài từ 10 đến 12 ký tự
- `Date Bounds`:
  - `EndDate` phải lớn hơn `StartDate`
  - nếu không hợp lệ, nút `Save` phải bị disable hoặc không cho submit

## 7. Đánh giá mức độ phù hợp

- Các phần về login, phân quyền, CRUD, tìm kiếm, trạng thái dữ liệu, xóa phòng, trùng lịch, giá cuối tuần, checkout, hủy phòng và validation **phù hợp với requirement**.
- Các yếu tố như gradient sidebar, card layout, pagination, icon action và hiệu ứng hiển thị chỉ nên xem là **cải tiến UI**.
- Khi triển khai, cần ưu tiên đúng nghiệp vụ trước, sau đó mới tối ưu giao diện.
