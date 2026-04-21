using LWMS.Domain.Common;
using LWMS.Domain.ValueObjects;

namespace LWMS.Domain.Entities;

/// <summary>
/// 🗺️ FEE CONFIG — Cấu hình bảng giá cước (route-based + versioning).
/// Cho phép thay đổi giá theo thời gian mà không cần sửa code.
/// </summary>
public class FeeConfig : BaseEntity
{
    /// <summary>Loại dịch vụ áp dụng (Standard, Express,...)</summary>
    public Guid ServiceTypeId { get; set; }

    /// <summary>Vùng áp dụng (Nội tỉnh, Liên tỉnh,...)</summary>
    public Guid ZoneId { get; set; }

    // ── Route-based pricing ──

    /// <summary>Mã tỉnh gửi (null = áp dụng tất cả)</summary>
    public string? ProvinceFrom { get; set; }

    /// <summary>Mã tỉnh nhận (null = áp dụng tất cả)</summary>
    public string? ProvinceTo { get; set; }

    // ── Weight range ──

    /// <summary>Mức cân nặng tối thiểu (gram)</summary>
    public decimal MinWeight { get; set; }

    /// <summary>Mức cân nặng tối đa (gram)</summary>
    public decimal MaxWeight { get; set; }

    // ── Pricing ──

    /// <summary>Giá cước cơ bản</summary>
    public Money BasePrice { get; set; } = null!;

    /// <summary>Phí cộng thêm cho mỗi kg vượt mức</summary>
    public Money ExtraPricePerKg { get; set; } = null!;

    // ── Rule management ──

    /// <summary>Ưu tiên (số nhỏ = ưu tiên cao) — khi nhiều rule match</summary>
    public int Priority { get; set; } = 0;

    /// <summary>Bắt đầu có hiệu lực</summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>Hết hiệu lực (null = vô thời hạn)</summary>
    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;
}