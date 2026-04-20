using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

/// <summary>
/// Thực thể Kệ hàng (Warehouse Rack).
/// Đại diện cho một vị trí vật lý trong Hub để lưu trữ Parcel hoặc Bag.
/// </summary>
public class Rack : BaseEntity
{
    /// <summary>
    /// Hub sở hữu kệ này.
    /// </summary>
    public Guid HubId { get; set; }

    /// <summary>
    /// Mã kệ (VD: HN-01-A-01 -> Hub Hà Nội, Khu A, Kệ 01).
    /// </summary>
    public string RackCode { get; set; } = string.Empty;

    /// <summary>
    /// Sức chứa tối đa (số lượng kiện hoặc số kg).
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Loại hàng hóa kệ này chấp nhận (VD: Hàng dễ vỡ, Hàng cồng kềnh).
    /// </summary>
    public string RackType { get; set; } = "GENERAL";

    public bool IsFull => CurrentUsage >= Capacity;

    /// <summary>
    /// Số lượng bưu kiện hiện có trên kệ (Logic nghiệp vụ sẽ update số này).
    /// </summary>
    public int CurrentUsage { get; set; }
}