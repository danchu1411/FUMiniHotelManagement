# DATABASE SCHEMA SPECIFICATION: FUMINIHOTELMANAGEMENT

Tài liệu đặc tả cấu trúc Cơ sở dữ liệu được biên dịch và tối ưu hóa chuyên biệt cho AI Agent (LLM, RAG, Code Generator). Tài liệu sử dụng cấu trúc bảng tường minh, chỉ rõ kiểu dữ liệu gốc, ràng buộc toàn vẹn và các quy tắc đồng bộ (Cascade).

---

## 1. Danh sách các bảng chi tiết (Entity Definitions)

### 1.1. Bảng `Customer` (Thông tin khách hàng)
* **Mô tả:** Lưu trữ thông tin tài khoản và thông tin cá nhân của khách hàng.

| Tên cột (Column) | Kiểu dữ liệu (Type) | Nullable | Khóa / Chỉ mục (Key/Index) | Thuộc tính đặc biệt / Ràng buộc |
| :--- | :--- | :--- | :--- | :--- |
| `CustomerID` | `int` | `NOT NULL` | **PK (Clustered)** | `IDENTITY(3,1)` (Bắt đầu từ 3, tăng 1) |
| `CustomerFullName` | `nvarchar(50)` | `NULL` | | |
| `Telephone` | `nvarchar(12)` | `NULL` | | |
| `EmailAddress` | `nvarchar(50)` | `NOT NULL` | **UQ (Unique Key)** | Ràng buộc duy nhất phi cụm (Nonclustered Index) |
| `CustomerBirthday` | `date` | `NULL` | | |
| `CustomerStatus` | `tinyint` | `NULL` | | Quy ước trạng thái tài khoản |
| `Password` | `nvarchar(50)` | `NULL` | | Mã hóa/Chuỗi mật khẩu đăng nhập |

### 1.2. Bảng `RoomType` (Danh mục loại phòng)
* **Mô tả:** Định nghĩa các loại phòng có trong hệ thống khách sạn.

| Tên cột (Column) | Kiểu dữ liệu (Type) | Nullable | Khóa / Chỉ mục (Key/Index) | Thuộc tính đặc biệt / Ràng buộc |
| :--- | :--- | :--- | :--- | :--- |
| `RoomTypeID` | `int` | `NOT NULL` | **PK (Clustered)** | `IDENTITY(1,1)` (Bắt đầu từ 1, tăng 1) |
| `RoomTypeName` | `nvarchar(50)` | `NOT NULL` | | Tên phân loại (ví dụ: Standard, Suite, Deluxe) |
| `TypeDescription` | `nvarchar(250)` | `NULL` | | Mô tả chi tiết về đặc quyền loại phòng |
| `TypeNote` | `nvarchar(250)` | `NULL` | | Ghi chú bổ sung |

### 1.3. Bảng `RoomInformation` (Thông tin phòng)
* **Mô tả:** Quản lý danh sách các phòng vật lý cụ thể trong khách sạn.

| Tên cột (Column) | Kiểu dữ liệu (Type) | Nullable | Khóa / Chỉ mục (Key/Index) | Thuộc tính đặc biệt / Ràng buộc |
| :--- | :--- | :--- | :--- | :--- |
| `RoomID` | `int` | `NOT NULL` | **PK (Clustered)** | `IDENTITY(1,1)` (Bắt đầu từ 1, tăng 1) |
| `RoomNumber` | `nvarchar(50)` | `NOT NULL` | | Số phòng hiển thị trên cửa |
| `RoomDetailDescription`| `nvarchar(220)` | `NULL` | | Mô tả hiện trạng phòng |
| `RoomMaxCapacity` | `int` | `NULL` | | Sức chứa người tối đa |
| `RoomTypeID` | `int` | `NOT NULL` | **FK** | Tham chiếu đến `RoomType(RoomTypeID)` |
| `RoomStatus` | `tinyint` | `NULL` | | Quy ước trạng thái vận hành của phòng |
| `RoomPricePerDay` | `money` | `NULL` | | Giá thuê gốc một ngày |

### 1.4. Bảng `BookingReservation` (Hóa đơn đặt phòng tổng thể)
* **Mô tả:** Ghi nhận thông tin tổng quan của một giao dịch đặt phòng do khách hàng thực hiện.

| Tên cột (Column) | Kiểu dữ liệu (Type) | Nullable | Khóa / Chỉ mục (Key/Index) | Thuộc tính đặc biệt / Ràng buộc |
| :--- | :--- | :--- | :--- | :--- |
| `BookingReservationID` | `int` | `NOT NULL` | **PK (Clustered)** | Mã định danh đơn đặt phòng (Không tự tăng) |
| `BookingDate` | `date` | `NULL` | | Ngày thực hiện giao dịch đặt đơn |
| `TotalPrice` | `money` | `NULL` | | Tổng chi phí của toàn bộ đơn đặt phòng |
| `CustomerID` | `int` | `NOT NULL` | **FK** | Tham chiếu đến `Customer(CustomerID)` |
| `BookingStatus` | `tinyint` | `NULL` | | Quy ước trạng thái xử lý đơn đặt |

### 1.5. Bảng `BookingDetail` (Chi tiết phòng được đặt)
* **Mô tả:** Bảng trung gian giải quyết quan hệ Nhiều - Nhiều (N:M) giữa `BookingReservation` và `RoomInformation`. Ghi nhận chi tiết từng phòng được đặt trong một đơn.

| Tên cột (Column) | Kiểu dữ liệu (Type) | Nullable | Khóa / Chỉ mục (Key/Index) | Thuộc tính đặc biệt / Ràng buộc |
| :--- | :--- | :--- | :--- | :--- |
| `BookingReservationID` | `int` | `NOT NULL` | **PK hỗn hợp / FK** | Tham chiếu đến `BookingReservation(BookingReservationID)` |
| `RoomID` | `int` | `NOT NULL` | **PK hỗn hợp / FK** | Tham chiếu đến `RoomInformation(RoomID)` |
| `StartDate` | `date` | `NOT NULL` | | Ngày nhận phòng dự kiến (Check-in Date) |
| `EndDate` | `date` | `NOT NULL` | | Ngày trả phòng dự kiến (Check-out Date) |
| `ActualPrice` | `money` | `NULL` | | Giá thuê thực tế áp dụng cho phòng tại thời điểm đặt |

---

## 2. Ràng buộc Khóa ngoại và Toàn vẹn dữ liệu (Relationships & Constraints)

Tất cả các mối quan hệ thực thể trong hệ thống này đều áp dụng quy tắc đồng bộ hóa dữ liệu tự động (`CASCADE`).

[Customer] 1 -------> N [BookingReservation] 1 -------> N [BookingDetail] N <------- 1 [RoomInformation] N <------- 1 [RoomType]


### 2.1. Chi tiết các liên kết Khóa ngoại (Foreign Key Constraints)

1. **`FK_BookingReservation_Customer`**
   * **Bảng con:** `BookingReservation(CustomerID)` $\rightarrow$ **Bảng cha:** `Customer(CustomerID)`
   * **Hành vi ràng buộc:** `ON UPDATE CASCADE`, `ON DELETE CASCADE`

2. **`FK_RoomInformation_RoomType`**
   * **Bảng con:** `RoomInformation(RoomTypeID)` $\rightarrow$ **Bảng cha:** `RoomType(RoomTypeID)`
   * **Hành vi ràng buộc:** `ON UPDATE CASCADE`, `ON DELETE CASCADE`

3. **`FK_BookingDetail_BookingReservation`**
   * **Bảng con:** `BookingDetail(BookingReservationID)` $\rightarrow$ **Bảng cha:** `BookingReservation(BookingReservationID)`
   * **Hành vi ràng buộc:** `ON UPDATE CASCADE`, `ON DELETE CASCADE`

4. **`FK_BookingDetail_RoomInformation`**
   * **Bảng con:** `BookingDetail(RoomID)` $\rightarrow$ **Bảng cha:** `RoomInformation(RoomID)`
   * **Hành vi ràng buộc:** `ON UPDATE CASCADE`, `ON DELETE CASCADE`

### 2.2. Các chỉ mục duy nhất (Unique Indexes)
* **`UQ__Customer__49A147405153121E`:** Chỉ mục duy nhất phi cụm trên trường `EmailAddress` của b