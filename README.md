# 📦 LWMS - Logistics Warehouse Management System

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-blue?style=for-the-badge)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![License](https://img.shields.io/badge/Status-Production_Ready-success?style=for-the-badge)](https://github.com/)

**LWMS** là một hệ thống quản lý kho vận và hành trình bưu kiện (Logistics) doanh nghiệp, được xây dựng với mục tiêu cung cấp giải pháp vận hành tin cậy, bảo mật và hiệu suất cao. Hệ thống quản lý toàn bộ vòng đời bưu kiện từ khi tạo đơn, nhập kho, đóng bao trung chuyển cho đến khi giao hàng và đối soát tài chính.

---

## 🌟 Tính Năng Cốt Lõi (Core Features)

### 1. Quản Lý Vòng Đời Bưu Kiện (Parcel Lifecycle)
- **State Machine Core**: Kiểm soát 18 trạng thái nghiệp vụ chặt chẽ từ lõi Domain, ngăn chặn lỗi cập nhật sai luồng vận hành.
- **Smart Tracking**: Hệ thống log sự kiện (TrackingLog) chi tiết cho từng thay đổi nhỏ nhất.
- **SLA Alerts**: Tự động cảnh báo các đơn hàng chậm trễ hoặc tồn đọng tại Hub quá thời gian quy định.

### 2. Quản Lý Bao Gói (Bagging System)
- Khả năng gộp hàng nghìn đơn hàng nhỏ vào các bao tải vận chuyển lớn (Bag).
- Quản lý qua **Seal Code** bảo mật và quy trình Scan-in/Scan-out chuyên nghiệp.

### 3. Đối Soát Tài Chính COD (Financial Tracking)
- Theo dõi dòng tiền thu hộ (COD) xuyên suốt từ Shipper -> Hub -> Merchant.
- Quy trình **Settlement** (Đối soát) minh bạch, quản lý trạng thái nộp tiền và thanh toán.

### 4. Enterprise Ready Integration
- **Bulk Upload**: Xử lý hàng nghìn đơn hàng qua CSV bằng `CsvHelper`.
- **Excel Reporting**: Xuất báo cáo hoạt động và tài chính định dạng chuyên nghiệp qua `ClosedXML`.
- **Labels Printing**: Tạo nhãn vận đơn (Label) chuẩn nhiệt (100x150mm) sắc nét qua `QuestPDF`.

---

## 🏗️ Kiến Trúc Hệ Thống (Architecture)

Dự án áp dụng **Clean Architecture** kết hợp với **Domain-Driven Design (DDD)** để đảm bảo khả năng mở rộng và bảo trì lâu dài:

- **Domain Layer**: Chứa các Entity cốt lõi, Value Objects (Money, Address) và nghiệp vụ trọng tâm (State Machine).
- **Application Layer**: Triển khai luồng xử lý qua **CQRS (MediatR)**, Validation chặt chẽ với **FluentValidation**.
- **Infrastructure Layer**: Quản lý Persistence (EF Core với SQL Server), Log (Serilog) và các Integration Services.
- **API Layer**: Cung cấp RESTful API, tài liệu Swagger và hệ thống Middleware mạnh mẽ.

---

## 🛡️ Hệ Thống Hardening (Production-Ready)

Hệ thống đã được nâng cấp lên tiêu chuẩn Production tại Phase 4:
- **Security**: Tích hợp JWT Authentication, phân quyền đa lớp RBAC và cơ chế Ownership Check (Kiểm tra quyền sở hữu dữ liệu).
- **Global Error Handling**: Middleware xử lý exception tập trung, trả về lỗi chuẩn **RFC 7807 (Problem Details)**.
- **Observability**: Tracking request thông qua **Correlation ID**, giúp truy vết log lỗi xuyên suốt các service.
- **Rate Limiting**: Bảo vệ API khỏi các cuộc tấn công spam hoặc quá tải từ phía Client.
- **Concurrency Control**: Sử dụng `RowVersion` để xử lý xung đột dữ liệu đồng thời (Race Condition).

---

## 🛠️ Công Nghệ Sử Dụng (Tech Stack)

| Thành phần | Công nghệ |
| :--- | :--- |
| **Backend** | .NET 10.0 |
| **Database** | SQL Server (Entity Framework Core) |
| **Logic Flow** | MediatR (CQRS Pattern) |
| **Validation** | FluentValidation |
| **Reporting** | ClosedXML (Excel), QuestPDF (PDF), CsvHelper (CSV) |
| **Logging** | Serilog |
| **API Doc** | Swagger / OpenAPI |

---

## 🚀 Hướng Dẫn Cài Đặt (Getting Started)

### 1. Yêu cầu hệ thống
- .NET 10 SDK
- SQL Server (LocalDB hoặc Server thực tế)

### 2. Cấu hình
Cập nhật chuỗi kết nối Database trong file `src/LWMS.API/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=LWMS;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 3. Chạy ứng dụng
```bash
dotnet build
dotnet run --project src/LWMS.API
```

---

## 🧪 Kiểm Thử (Testing)

Hệ thống đi kèm với bộ Script kiểm thử tích hợp (Integration Test) tự động bằng PowerShell, bao quát 12 Test Case QA chuyên sâu:
- Happy Path (Tạo đơn, Nhập kho, Giao hàng).
- Edge Cases (Dữ liệu lỗi, Logic sai luồng).
- Concurrency (Kiểm tra xung đột dữ liệu đồng thời).

**Chạy test:**
```powershell
powershell -File docs/test/FULL_SYSTEM_TEST_AUTOMATION.ps1
```

---

## ✉️ Liên Hệ & Đóng Góp
Dự án được phát triển với tinh thần mã nguồn mở và liên tục cải tiến. Mọi đóng góp xin vui lòng gửi Pull Request hoặc tạo Issue.

**LWMS Team - Build for Reliability.**
