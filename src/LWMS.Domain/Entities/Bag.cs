using LWMS.Domain.Common;
using LWMS.Domain.Enums;

namespace LWMS.Domain.Entities;

/// <summary>
/// 🟨 BAG — Bao/Túi vận chuyển tuyến (Hub A → Hub B).
/// Chứa nhiều BagItem, mỗi BagItem tham chiếu 1 Parcel.
/// </summary>
public class Bag : BaseEntity
{
    /// <summary>Mã bao duy nhất</summary>
    public string BagCode { get; set; } = string.Empty;

    /// <summary>Hub gửi đi</summary>
    public Guid FromHubId { get; set; }

    /// <summary>Hub nhận</summary>
    public Guid ToHubId { get; set; }

    /// <summary>Trạng thái: Open, Sealed, InTransit, Received</summary>
    public BagStatus Status { get; set; } = BagStatus.Open;

    /// <summary>Thời điểm niêm phong bao (sealed = không thêm parcel được nữa)</summary>
    public DateTime? SealedAt { get; set; }

    /// <summary>Thời điểm mở bao tại Hub đích</summary>
    public DateTime? OpenedAt { get; set; }

    /// <summary>Số lượng bưu kiện trong bao (cache — tránh COUNT mỗi lần)</summary>
    public int ParcelCount { get; set; }

    /// <summary>Danh sách BagItem (navigation property)</summary>
    public List<BagItem> Packages { get; set; } = new();
}