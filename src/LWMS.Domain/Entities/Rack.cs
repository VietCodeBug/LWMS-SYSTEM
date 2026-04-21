using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

/// <summary>
/// 🏗️ RACK — Kệ hàng trong Hub (vị trí vật lý).
/// Quản lý capacity và trạng thái khóa kệ.
/// </summary>
public class Rack : BaseEntity
{
    /// <summary>Hub sở hữu kệ này</summary>
    public Guid HubId { get; set; }

    /// <summary>Mã kệ (VD: HN-01-A-01 → Hub HN, Khu A, Kệ 01)</summary>
    public string RackCode { get; set; } = string.Empty;

    /// <summary>Sức chứa tối đa (số lượng kiện)</summary>
    public int Capacity { get; set; }

    /// <summary>Loại hàng kệ chấp nhận: GENERAL / FRAGILE / OVERSIZED</summary>
    public string RackType { get; set; } = "GENERAL";

    /// <summary>Khu vực trong kho: A / B / C (phân vùng nội bộ)</summary>
    public string? ZoneInWarehouse { get; set; }

    /// <summary>Kệ đang bị khóa (bảo trì hoặc đầy)</summary>
    public bool IsLocked { get; set; } = false;

    /// <summary>Số lượng bưu kiện hiện có trên kệ</summary>
    public int CurrentUsage { get; set; }

    /// <summary>Computed: kệ đã đầy chưa (không lưu DB)</summary>
    public bool IsFull => CurrentUsage >= Capacity;
}