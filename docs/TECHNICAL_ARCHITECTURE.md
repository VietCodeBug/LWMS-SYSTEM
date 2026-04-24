# 📦 TÀI LIỆU CẤU TRÚC KỸ THUẬT & KIẾN TRÚC HỆ THỐNG LWMS (Version 2.1)

Tài liệu này (cập nhật mới nhất 24/04/2026) đóng vai trò là "Sổ tay Kỹ Thuật" (Technical Blueprint) của dự án Logistics Warehouse Management System (LWMS). Tài liệu giải thích cặn kẽ về công nghệ sử dụng, kiến trúc source code (Clean Architecture), và cấu trúc thư mục nhằm phục vụ việc onboard người mới và nâng cấp bảo trì trong tương lai.

---

## 🛠️ 1. STACK CÔNG NGHỆ VÀ THƯ VIỆN ĐANG SỬ DỤNG
Dự án được xây dựng dựa trên hệ sinh thái .NET hiện đại, định hướng Production-Ready với hiệu năng và độ bảo mật cao:
- **Ngôn ngữ & Framework:** C# 12, **.NET 10.0**.
- **Cơ sở dữ liệu:** Microsoft SQL Server.
- **ORM (Object-Relational Mapping):** Entity Framework Core 8/10 (Code-First Approach).
- **Kiến trúc hệ thống:** Clean Architecture, Domain-Driven Design (DDD), CQRS Pattern.
- **Thư viện lõi (Core NuGet Packages):**
  - `MediatR`: Xử lý phân tách Commands (Ghi) và Queries (Đọc) qua Pipeline Behaviors.
  - `FluentValidation`: Viết các luật (Rules) kiểm tra dữ liệu đầu vào (Input Validation) tách biệt khỏi logic.
  - `Serilog`: Framework ghi log có cấu trúc mạnh mẽ, tích hợp khả năng xuất log ra File/Console theo ngày.
  - `JWT Bearer`: Xử lý xác thực người dùng (Authentication) và phân quyền (RBAC).
  - Thư viện khác: `CsvHelper` (để xử lý File Bulk Upload CSV), `ClosedXML` (xuất báo cáo Excel), `QuestPDF` (tạo nhãn in nhiệt PDF).

---

## 📂 2. PHÂN TÍCH CẤU TRÚC THƯ MỤC SOURCE CODE

Dự án tuân thủ nghiêm ngặt **Clean Architecture** chia thành 4 Tầng (Layers). Dưới đây là phân tích chi tiết từng folder trong hệ thống:

### 🟩 Lớp Lõi: `src/LWMS.Domain/` (Trái tim của hệ thống)
*Không phụ thuộc vào bất kỳ công nghệ hay framework nào khác ngoài .NET cơ bản.*
- **`/Entities/`**: Chứa toàn bộ các mô hình dữ liệu (Bảng trong DB) như `Parcel.cs`, `Bag.cs`, `CodRecord.cs`. Đặc điểm quan trọng là mọi Entity đều kế thừa `BaseEntity` (hỗ trợ Audit tự động và RowVersion cho Concurrency Lock).
- **`/Enums/`**: Nơi quy định các hằng số trạng thái. Đặc biệt là file `ParcelStatus.cs` (chứa 18 trạng thái chuẩn của 1 bưu kiện Logistics).
- **`/Services/`**: Chứa các Domain Services (logic tính toán thuần). Quan trọng nhất là file `ParcelStateMachine.cs` — cỗ máy trạng thái bắt buộc mọi bưu kiện phải đi theo đúng luồng định sẵn, không thể "nhảy cóc" trạng thái (ví dụ từ `Created` không thể sang `Delivered`).
- **`/ValueObjects/`**: Các object không có Identity (Id) độc lập, ví dụ như `Money` (tiền tệ), `Address` (địa chỉ).
- **`/Common/`**: Các Interfaces chuẩn như `IMustHaveMerchant` (để đánh dấu Tenant/Merchant).

### 🟦 Lớp Điều Phối: `src/LWMS.Application/` (CQRS Use Cases)
*Giao tiếp giữa lớp API và lớp Lõi. Chứa toàn bộ "Thao tác" (Business Logic) của hệ thống.*
- **`/Common/Behaviors/`**: Rất Quan Trọng! Đây là các Pipeline chặn giữa mọi Request.
  - `LoggingBehavior.cs`: Tự động log lại "START" và "SUCCESS" cùng thời gian chạy cho mọi API.
  - `ValidationBehavior.cs`: Tự động chặn Request lại nếu input (định nghĩa bởi FluentValidation) bị sai.
- **Thư mục theo Entity (VD: `/Bags/`, `/Parcels/`, `/Cod/`):** Chứa các Use Case thực tế.
  - **`.../Commands/`**: Chứa các hành động thay đổi dữ liệu (Write). Ví dụ: `CreateParcelCommand` (Tạo đơn), `SettleCodCommand` (Đối soát COD), `ReAssignShipperCommand` (Gán lại Shipper).
  - **`.../Queries/`**: Chứa các hành động đọc dữ liệu (Read). Ví dụ: Lấy chi tiết Tracking của đơn.

### 🟨 Lớp Hạ Tầng: `src/LWMS.Infrastructure/` (Giao tiếp với Hệ thống bên ngoài)
*Nơi chứa mọi thứ liên quan đến Database, File System, API kết nối bên thứ 3.*
- **`/Data/AppDbContext.cs`**: Timeline chính giao tiếp với SQL Server. Tại đây có xử lý **Global Query Filter** để tự động gắn `MerchantId` cho mọi câu Query, đảm bảo dữ liệu Merchant A không bao giờ lộ cho Merchant B.
- **`/Data/Configurations/RelationshipsConfiguration.cs`**: Mapping chi tiết các bảng, định nghĩa Khóa chính, Khóa ngoại (1-N, 1-1, N-N), tránh tạo Shadow Properties.
- **`/Migrations/`**: Chứa lịch sử các file thay đổi Database.
- **`/Services/`**: Triển khai các Interface giao tiếp mạng như `JwtService` (sinh Token).

### 🟥 Lớp Trình Bày: `src/LWMS.API/` (Cổng Giao Tiếp Mạng)
*Chỉ chứa Logic điều phối HTTP, tuyệt đối không chứa Business Logic.*
- **`/Endpoints/`**: Sử dụng công nghệ **Minimal APIs** của .NET để tối ưu hiệu năng. Chứa các file như `BagEndpoints.cs`, `CodEndpoints.cs` (Được bổ sung ở Phase 4).
- **`/Controllers/`**: Các REST API Controller truyền thống.
- **`/Middlewares/`**: Chứa các thành phần chặn HTTP Requests:
  - `ExceptionMiddleware.cs`: Bắt tất cả lỗi crash (Exception) chưa xử lý, đóng gói thành chuẩn lỗi **RFC 7807 (Problem Details)** trả về cho Client, giúp Server không bao giờ sập.
  - `CorrelationIdMiddleware.cs`: Sinh ID duy nhất cho mỗi Request để dễ dàng tra cứu lỗi (Traceability).

### 🧪 Lớp Kiểm Thử: `src/LWMS.Tests/` & `tests/`
*Tự động hóa kiểm tra mã nguồn để chặn lỗi hồi quy (Regression bugs).*
- Chứa các Unit Tests (VD: Test State Machine ở `tests/LWMS.Domain.UnitTests/ParcelStateMachineTests.cs`) và Integration Tests. Đảm bảo Core Rule không bị phá vỡ.

---

## 🚦 3. QUY TẮC NGHIỆP VỤ (BUSINESS RULES) QUAN TRỌNG ĐỂ NÂNG CẤP TƯƠNG LAI

Nếu có nâng cấp hệ thống, lập trình viên cần **đặc biệt chú ý** các rule sau (nếu vi phạm, hệ thống Logistics sẽ bị lỗi nặng):

1. **State Machine Lock:** Bưu kiện (Parcel) KHÔNG ĐƯỢC PHÉP thay đổi trạng thái qua các hàm Set cứng `parcel.Status = ...`. Bắt buộc phải dùng hàm `parcel.ChangeStatusWithLog()` để validate qua `ParcelStateMachine`.
2. **Append-Only Logs:** Các bảng `tracking_logs` và `audit_logs` có tính chất Append-Only. Tuyệt đối không viết API cho phép Update hay Delete các bảng này (Dữ liệu Tracking là lịch sử vận hành, giống như Blockchain).
3. **Double COD Protection:** Khi gọi API Settle (đối soát) tiền COD, cần đảm bảo tính Idempotency (không thu trùng tiền). Logic khóa (Locking) đang được quản lý bởi trạng thái của `CodRecord`.
4. **Terminal States:** Trạng thái `DELIVERED` và `RETURNED` là trạng thái cuối vòng đời. Bưu kiện lọt vào đây sẽ không thể thay đổi được nữa.
5. **RowVersion / Concurrency:** Mọi Entity có `RowVersion`. Nếu 2 shipper cùng ấn "Giao thành công" một đơn tại cùng 1 mili-giây, hệ thống EF Core sẽ ném ra lỗi `DbUpdateConcurrencyException`. Luôn phải catch lỗi này ở tầng Application.

---

## 🚀 4. MỞ RỘNG (SCALABILITY GHI CHÚ)

Để dự án LWMS chịu tải (Scale) khi công ty mở rộng lên hàng triệu đơn/ngày, cần lên kế hoạch chuẩn bị tích hợp:
- **Message Broker (RabbitMQ / Kafka):** Dùng để xử lý bất đồng bộ các luồng thông báo (VD: Khi Đơn Hàng Giao Thành Công -> Bắn Event -> Service Kế toán nhận Event để cộng tiền). Không nên chạy đồng bộ (Synchronous) trong MediatR nữa.
- **Redis Cache:** Gắn đè lên tầng `Queries` trong `Application Layer` để Cache danh sách Bảng Giá (`FeeConfig`), Danh sách Tỉnh Thành (`Zones`).
- **Elasticsearch:** Chuyển `TrackingLog` từ SQL Server sang Elastic để tối ưu tốc độ Search Lịch sử bưu kiện.
