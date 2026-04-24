# BÁO CÁO TỔNG KẾT: CHUYỂN ĐỔI TI ĐB CLOUD & ỔN ĐỊNH HỆ THỐNG
**Thời gian:** 24/04/2026 11:42
**Người thực hiện:** Antigravity (AI Assistant)

---

## 📌 1. TỔNG QUAN NHIỆM VỤ
Hoàn tất quá trình chuyển đổi hạ tầng LWMS Logistics từ MSSQL Local sang **TiDB Cloud (MySQL)** và ổn định lớp ứng dụng (Authentication, Data Seeding, Infrastructure Cleanup).

## 🚀 2. CÁC THAY ĐỔI VỆ HẠ TẦNG (INFRASTRUCTURE)
- **Database**: Loại bỏ 100% MSSQL. Toàn bộ logic hiện tại chạy trực tiếp trên TiDB Cloud Gateway.
- **Dọn dẹp mã nguồn (Cleanup)**:
  - Xóa bỏ `CloudSyncWorker`: Ngừng việc đồng bộ dữ liệu thừa giữa 2 DB.
  - Xóa bỏ `DataMigrationController`: Đóng các API di trú dữ liệu cũ.
- **Dependency Injection**: Refactor file `DependencyInjection.cs` để sử dụng `Pomelo.EntityFrameworkCore.MySql` thay thế cho SQL Server.

## 🔑 3. XÁC THỰC & BẢO MẬT (AUTHENTICATION)
- **Login Recovery**: Khắc phục triệt để lỗi 401 Unauthorized khi đăng nhập.
- **Username Flexibility**: Cho phép đăng nhập linh hoạt bằng cả **Số điện thoại** hoặc **Mã nhân viên (EmployeeCode)**.
- **Password Hashing**: Chuyển đổi mã hóa mật khẩu sang chuẩn **BCrypt.Net** để đảm bảo tính thương thích và bảo mật.
- **JWT Lifetime**:
  - Sửa lỗi Token hết hạn sau 1 phút.
  - Thiết lập thời hạn Token mới là **24 giờ**.
  - Đồng bộ thời gian hệ thống sang **UTC (UtcNow)** để tránh lỗi Token invalid do lệch múi giờ.

## 📦 4. NẠP DỮ LIỆU MẪU (SEED DATA)
- **Tài khoản test**: Khởi tạo 4 tài khoản mẫu (`ADMIN`, `STAFF-HN`, `SHIPPER-HCM`, `MANAGER-HCM`) với mật khẩu chung là **`Admin@123`**.
- **10 Đơn hàng thực tế**:
  - Nạp 10 đơn hàng (Parcels) dùng dữ liệu thật: Tên người Việt, Địa chỉ cụ thể Hà Nội/HCM, Cân nặng, Tiền thu hộ (COD).
  - Trạng thái đa dạng: `Created`, `Picked`, `ArrivedHub`, `OutForDelivery`, `Delivered`, `FailedDelivery`, `Returning`, `Returned`.
  - Toàn bộ bưu kiện được đánh dấu **`[TEST DATA]`** trong cột Notes để quản lý.

## ✅ 5. TRẠNG THÁI HIỆN TẠI
- **Database Status**: Kết nối TiDB Cloud ổn định.
- **Functional Status**: API Đăng nhập, API lấy danh sách Hubs, Merchants và Parcels hoạt động tốt 100%.
- **Clean Code**: Loại bỏ các Exception che dấu lỗi thật, giúp quá trình debug dễ dàng hơn.

---
*Báo cáo được tạo tự động nhằm ghi lại các workshop trong phiên làm việc ngày 24/04/2026.*
