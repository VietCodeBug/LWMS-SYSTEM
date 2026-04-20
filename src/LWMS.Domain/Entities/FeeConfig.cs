using LWMS.Domain.Common;
using LWMS.Domain.ValueObjects;

namespace LWMS.Domain.Entities;

/// <summary>
/// Thực thể cấu hình phí (Fee Configuration).
/// Đây là "Trái tim" của logic tính tiền, cho phép thay đổi giá mà không cần code lại.
/// </summary>
public class FeeConfig : BaseEntity
{
    /// <summary>
    /// Loại dịch vụ áp dụng (Standard, Express,...).
    /// </summary>
    public Guid ServiceTypeId { get; set; }

    /// <summary>
    /// Vùng áp dụng (Nội tỉnh, Liên tỉnh,...).
    /// </summary>
    public Guid ZoneId { get; set; }

    /// <summary>
    /// Mức cân nặng tối thiểu (gram).
    /// </summary>
    public decimal MinWeight { get; set; }

    /// <summary>
    /// Mức cân nặng tối đa (gram).
    /// </summary>
    public decimal MaxWeight { get; set; }

    /// <summary>
    /// Giá cước cơ bản cho khoảng cân nặng này.
    /// </summary>
    public Money BasePrice { get; set; } = null!;

    /// <summary>
    /// Phí cộng thêm cho mỗi kg vượt mức (nếu có).
    /// </summary>
    public Money ExtraPricePerKg { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}