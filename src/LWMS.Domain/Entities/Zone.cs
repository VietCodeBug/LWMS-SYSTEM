using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

/// <summary>
/// Thực thể vùng địa lý (Shipping Zone).
/// Dùng để phân loại khu vực tính phí (VD: Nội tỉnh, Liên tỉnh, Vùng sâu vùng xa).
/// </summary>
public class Zone : BaseEntity
{
    /// <summary>
    /// Tên vùng (VD: Urban, Rural, Highlands).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Mã vùng để mapping nhanh (VD: ZONE_A, ZONE_B).
    /// </summary>
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Danh sách mã tỉnh/thành (ProvinceCode) thuộc vùng này.
    /// Dùng để tra cứu vùng dựa trên địa chỉ giao hàng.
    /// </summary>
    public List<string> ProvinceCodes { get; set; } = new();
}