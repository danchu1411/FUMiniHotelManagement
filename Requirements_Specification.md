# TÀI LIỆU ĐẶC TẢ YÊU CẦU PHẦN MỀM (REQUIREMENTS SPECIFICATION FOR AGENT/DEV)

Tài liệu này được cấu trúc tối ưu cho AI Agent và Lập trình viên để dễ dàng bóc tách thực thể, ánh xạ trạng thái và thiết kế các tầng xử lý dữ liệu (WPF, BLL, DAL/EF Core).

---

## 1. Cấu hình Hệ thống & Xác thực (System & Auth)

* [cite_start]**Cơ chế xác thực:** Bắt buộc đăng nhập bằng cặp thông tin `Email` và `Password` thông qua màn hình giao diện mặc định (`Login Window`)[cite: 53, 109].
* [cite_start]**Tài khoản Admin mặc định:** Hệ thống phải đọc dữ liệu từ tệp cấu hình `appsettings.json` để xác thực tài khoản quản trị[cite: 22, 73]:
    * [cite_start]**Email:** admin@FUMiniHotelSystem.com [cite: 22, 73]
    * [cite_start]**Mật khẩu:** @@abc123@@ [cite: 22, 73]

---

## 2. Phân quyền và Phạm vi Chức năng (Actor Capabilities)

### 2.1. Quyền hạn của Quản trị viên (Admin Role)
[cite_start]Admin có toàn quyền thực thi các thao tác **CRUD (Tạo, Đọc, Cập nhật, Xóa)** và **Tìm kiếm** trên các phân hệ sau[cite: 39, 46, 92, 100]:

* [cite_start]**Quản lý Khách hàng (`Customer`):** Thao tác trên các trường dữ liệu hồ sơ khách hàng[cite: 14, 16, 40, 66, 93]: *CustomerID, CustomerFullName, Telephone, EmailAddress, CustomerBirthday, CustomerStatus, Password*.
* [cite_start]**Quản lý Thông tin Phòng (`RoomInformation`):** Thao tác trên các trường dữ liệu danh mục phòng[cite: 11, 12, 41, 65, 94]: *RoomID, RoomNumber, RoomDetailDescription, RoomMaxCapacity, RoomTypeID, RoomStatus, RoomPricePerDay*.
    * **Sơ đồ phòng trực quan (Visual Room Map):** Giao diện lưới dạng Card thay đổi màu sắc động theo thời gian thực tương ứng với vòng đời phòng (Màu xanh: Phòng trống; Màu đỏ: Phòng có khách ở; Màu vàng: Phòng bẩn chờ dọn dẹp). Cho phép click đúp vào ô phòng để mở nhanh hộp thoại xử lý nghiệp vụ hoặc xem thông tin chi tiết.
* **Quản lý Đặt phòng & Vận hành quầy (Front-Desk Management):**
    * [cite_start]Theo dõi danh sách tổng thể `BookingReservation` và danh sách chi tiết phòng liên kết `BookingDetail`[cite: 21, 72, 95].
    * Cung cấp 2 nút xử lý nghiệp vụ trực tiếp trên UI: **"Xác nhận Check-in"** và **"Xác nhận Check-out"**.
* **Báo cáo Thống kê (`Statistic Report`):**
    * [cite_start]Lọc dữ liệu doanh thu/số liệu theo khoảng thời gian từ `StartDate` đến `EndDate`, sắp xếp kết quả trả về theo thứ tự thời gian **giảm dần**[cite: 42, 96].
    * **LINQ Analytics:** Tự động tính toán hiển thị 3 chỉ số tổng hợp gồm: Tổng doanh thu thực tế (Sum), Loại phòng thuê nhiều nhất (Mode), Khách hàng đặt phòng nhiều nhất (Top Buyer).

### 2.2. Quyền hạn của Khách hàng (Customer Role)
[cite_start]Giao diện biệt lập, chỉ hiển thị và cho phép thao tác các chức năng chủ động sau[cite: 43, 97]:

* [cite_start]**Hồ sơ & Bảo mật:** Cho phép khách hàng tự xem thông tin, cập nhật các trường dữ liệu cá nhân của chính mình và chủ động thay đổi mật khẩu tài khoản[cite: 44, 98].
* [cite_start]**Tìm phòng trống (Room Search):** Nhập khoảng thời gian mong muốn lưu trú (Ngày đến, Ngày đi) và chọn Loại phòng để lọc và hiển thị danh sách các phòng khả dụng[cite: 9, 63, 100].
* [cite_start]**Tự đặt phòng (Self-Booking):** Chọn phòng trống từ danh sách tìm kiếm và thực hiện tạo đơn đặt phòng nhanh[cite: 9, 63].
* **Hủy đơn đặt (Cancellation):** Chủ động bấm nút hủy các đơn phòng thỏa mãn điều kiện nghiệp vụ đang chờ.
* [cite_start]**Lịch sử & Hóa đơn điện tử:** Xem lại danh sách toàn bộ các đơn đặt phòng và chi tiết phòng mà cá nhân tài khoản này đã từng thực hiện trong hệ thống[cite: 45, 99].

---

## 3. Ánh xạ Trạng thái Dữ liệu (State Machine Mappings)

Hệ thống sử dụng các trường dữ liệu kiểu số nguyên có sẵn để ánh xạ trạng thái vận hành, **tuyệt đối không thêm trường mới vào cơ sở dữ liệu**.

### 3.1. Trạng thái Đơn đặt phòng (`BookingStatus` trong bảng `BookingReservation`)
* **`1` - Booked/Reserved:** Trạng thái mặc định khi Khách hàng hoặc Admin vừa tạo xong đơn đặt phòng.
* **`2` - Checked-In:** Kích hoạt khi Admin nhấn nút "Xác nhận Check-in". Khách chính thức lưu trú.
* **`3` - Checked-Out:** Kích hoạt sau khi Admin duyệt qua Hóa đơn và bấm "Xác nhận Check-out".
* **`0` - Cancelled:** Kích hoạt khi Khách hàng hoặc Admin thực hiện lệnh Hủy đơn phòng.

### 3.2. Trạng thái Vòng đời Phòng (`RoomStatus` trong bảng `RoomInformation`)
* **`1` - Available (Màu Xanh):** Phòng trống, sạch sẽ. Khách có thể tìm kiếm và đặt phòng này.
* **`2` - Occupied (Màu Đỏ):** Phòng đang có khách ở thực tế. Tự động chuyển sang mã này khi `BookingStatus = 2`.
* **`3` - Dirty (Màu Vàng):** Phòng bẩn cần dọn dẹp. Tự động chuyển sang mã này ngay khi Admin bấm "Xác nhận Check-out" (`BookingStatus = 3`). Khi dọn xong, nhân viên ấn nút "Xác nhận đã dọn dẹp" để chuyển về mã `1`.

---

## 4. Các Quy tắc Nghiệp vụ cốt lõi (Core Business Rules)

### 4.1. Logic Xóa Phòng (Room Deletion Policy)
* [cite_start]**Hard Delete (Xóa vật lý):** Hệ thống chỉ cho phép xóa hoàn toàn bản ghi thông tin phòng ra khỏi cơ sở dữ liệu nếu phòng đó chưa từng tham gia vào bất kỳ giao dịch thuê phòng nào (không tồn tại mã phòng này trong bảng chi tiết đặt phòng `BookingDetail`)[cite: 94].
* [cite_start]**Soft Delete (Xóa logic):** Trong trường hợp thông tin phòng đã tồn tại trong lịch sử chi tiết đặt phòng, hệ thống nghiêm cấm hành vi xóa vật lý[cite: 94]. [cite_start]Thay vào đó, hệ thống chỉ cập nhật chuyển trường `RoomStatus` sang mã trạng thái ngừng hoạt động/đã xóa[cite: 94].

### 4.2. Kiểm tra Trùng lịch Phòng (Anti-Overlapping Reservation)
Trước khi lưu một bản ghi vào `BookingDetail`, hệ thống bắt buộc phải thực hiện kiểm tra chéo thời gian dựa trên `RoomID`:
* **Điều kiện chặn:** Chặn hoàn toàn hành vi lưu dữ liệu nếu cặp thời gian nhập vào (`inputStartDate`, `inputEndDate`) giao thoa, trùng lặp hoặc nằm đè lên khoảng thời gian lưu trú (`StartDate` đến `EndDate`) của các đơn đặt phòng khác đang hoạt động thuộc chính phòng đó.
* **Hành vi:** Chặn lưu dữ liệu và thông báo lỗi chi tiết ra màn hình.

### 4.3. Tính Giá Phòng Động (Dynamic Weekend Pricing)
* Mức giá phòng thực tế (`ActualPrice` trong `BookingDetail`) được tính toán tự động tại tầng nghiệp vụ khi tạo đơn để tối ưu doanh thu.
* **Logic tính toán:** Nếu ngày bắt đầu ở (`StartDate`) rơi vào ngày **Thứ Bảy** hoặc **Chủ Nhật**, mức giá áp dụng thực tế tự động tính bằng giá gốc của phòng nhân với hệ số 1.2 (tăng 20%): `ActualPrice = RoomPricePerDay * 1.2`.
* Nếu `StartDate` rơi vào ngày hành chính trong tuần (Thứ Hai đến Thứ Sáu), giữ nguyên giá gốc của phòng: `ActualPrice = RoomPricePerDay`.

### 4.4. Quy trình Xuất hóa đơn Tổng hợp (Invoice / Billing)
* Khi Admin nhấn nút **"Xác nhận Check-out"**, hệ thống hiển thị một Popup dạng biểu mẫu Hóa đơn thanh toán (`Invoice`).
* **Công thức tính toán doanh thu tổng:** `TotalPrice = ActualPrice * Số ngày ở thực tế`.
* Dữ liệu chỉ được cập nhật chính thức xuống cơ sở dữ liệu (`BookingStatus = 3` và `RoomStatus = 3`) sau khi giao diện này được xác nhận hoàn tất.

### 4.5. Khách hàng tự Đặt và Hủy phòng trực tuyến
* **Ràng buộc đặt phòng:** Khi khách hàng thao tác đặt phòng, hệ thống tự động gán mã `CustomerID` của tài khoản đang đăng nhập. Khóa cứng không cho người dùng chọn hoặc sửa mã khách hàng trên UI.
* **Điều kiện Hủy đơn phòng:** Khách hàng chỉ có quyền bấm nút Hủy khi đơn đặt phòng đó có trạng thái `BookingStatus == 1` (Mới đặt chỗ, chưa Check-in).
* **Logic xử lý khi hủy đơn:** Hệ thống tự động chuyển `BookingStatus` về `0` (Cancelled), đồng thời giải phóng toàn bộ các phòng liên quan trong `BookingDetail` về lại trạng thái trống (`RoomStatus = 1`). Nếu trạng thái đã chuyển sang `2` hoặc `3`, nút hủy đơn trên giao diện khách hàng sẽ bị khóa mờ (Disable).

---

## 5. Quy tắc Giao diện & Kiểm thử dữ liệu (UI & Data Validation)

> ### ⚠️ Ràng buộc tương tác giao diện bắt buộc
> [cite_start]* **Popup Dialog:** Toàn bộ hành vi Tạo mới (Create) và Cập nhật (Update) dữ liệu của mọi phân hệ bắt buộc phải thực hiện trên một cửa sổ hộp thoại độc lập, không cho phép chỉnh sửa trực tiếp trên lưới `DataGrid`[cite: 47, 101].
> * **Confirmation Box:** Hành vi Xóa (Delete) dữ liệu bất kỳ luôn yêu cầu hiển thị hộp thoại thông báo xác nhận trước khi thực thi lệnh[cite: 47, 101].

### [cite_start]Quy tắc kiểm thử dữ liệu đầu vào (Validation Rules)[cite: 35, 84]:
* **EmailAddress:** Kiểm tra đúng định dạng RegEx mẫu và sử dụng LINQ quét qua DB để đảm bảo tính duy nhất, không trùng lặp giữa các khách hàng.
* **Telephone:** Bắt buộc chỉ chứa ký tự số, độ dài quy định từ 10 đến 12 ký tự.
* **Date Bounds:** Kiểm tra logic ngày tháng, ngày kết thúc đơn đặt (`EndDate`) bắt buộc phải lớn hơn ngày bắt đầu đơn đặt (`StartDate`). Nếu vi phạm, hệ thống tự động khóa mờ (Disable) nút lưu dữ liệu.