using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

/// <summary>
/// 📋 AUDIT LOG — Ghi nhận mọi thay đổi trong hệ thống (Append-only, toàn cục).
/// Không có FK cứng vì ghi log cho MỌI entity type (polymorphic).
/// </summary>
public class AuditLog : BaseEntity
{
    /// <summary>Id người thực hiện hành động</summary>
    public Guid ActorId { get; set; }

    /// <summary>Vai trò: Admin, Staff, System</summary>
    public string ActorRole { get; set; } = string.Empty;

    /// <summary>Hành động: CREATE, UPDATE, DELETE</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>Loại entity bị tác động: Parcel, User, Hub,...</summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>Id entity bị tác động</summary>
    public Guid EntityId { get; set; }

    /// <summary>Chi tiết thay đổi JSON: {"status":"CREATED→DELIVERED"}</summary>
    public string? Changes { get; set; }

    /// <summary>Địa chỉ IP client</summary>
    public string? IpAddress { get; set; }

    /// <summary>Correlation Id — trace xuyên suốt 1 request</summary>
    public string? CorrelationId { get; set; }

    /// <summary>User-Agent browser/app</summary>
    public string? UserAgent { get; set; }

    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}