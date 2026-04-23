# 📝 BÁO CÁO KIỂM THỬ — PHASE 3: APPLICATION LAYER & PIPELINES
**Dự án**: LWMS Logistics System  
**Test Area**: Logic nghiệp vụ & Application Pipelines (MediatR)  

---

## 1. MỤC TIÊU KIỂM THỬ
Xác nhận tính đúng đắn của các Behavior Pipepline trong Application layer:
*   **LoggingBehavior**: Đảm bảo mọi Request đi qua đều được ghi nhận (Log Start) và khi kết thúc sẽ ghi nhận kết quả và thời gian xử lý (Log Success / Error).

---

## 2. CHI TIẾT KỊCH BẢN KIỂM THỬ (TEST CASES)

### TC-01: Ghi nhận log (Start & Success) thông qua LoggingBehavior
*   **Input**:
    - Mô phỏng một Request (`TestRequest`).
    - Lấy thông tin `UserId` dạng `Guid?` từ hệ thống qua `ICurrentUserService`.
*   **Các bước thực hiện**: 
    1. Khởi tạo `LoggingBehavior` với `ILogger` được mock.
    2. Gọi hàm `Handle(request, next, cancellationToken)`.
    3. Xác nhận hàm `LogInformation` được gọi 1 lần với thông điệp chứa từ khóa "START".
    4. Xác nhận hàm `LogInformation` được gọi 1 lần với thông điệp chứa từ khóa "SUCCESS".
*   **Kết quả kỳ vọng**: Lớp LoggingInterceptor chặn request thành công, trích xuất chính xác UserID `(Guid?)` và đo đạc được thời gian thực thi (ElapsedMilliseconds).
*   **Kết quả thực tế**: 🔵 **PASSED** (100%).
    - Đã vượt qua khâu convert `Guid?` sang `string` mà không gặp lỗi Null Reference.
    - Test chạy nhanh (111ms).

---

## 3. KẾT LUẬN & GHI CHÚ
*   Kiến trúc Pipeline của MediatR hoạt động rất trơn tru, giúp tách biệt logic Cross-Cutting (Logging, Validation) ra khỏi Business logic của từng Command/Query.
*   Việc chuẩn hóa các ID về `Guid?` giúp cho việc giao tiếp giữa UI (Identity) và Backend (Domain Log) trong môi trường Production đảm bảo bảo mật và khả năng Scale up dễ dàng hơn.
