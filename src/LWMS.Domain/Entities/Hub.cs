using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

/// <summary>
/// 🏗️ HUB — Trung tâm kho vận (nút trong mạng lưới logistics).
/// Hub kết nối với: User, Bag, Rack, ShipperAssignment.
/// </summary>
public class Hub : BaseEntity
{
    /// <summary>Mã Hub duy nhất (VD: HN01, HCM02)</summary>
    public string HubCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    /// <summary>Mã tỉnh/thành (dùng để mapping Zone)</summary>
    public string ProvinceCode { get; set; } = string.Empty;

    /// <summary>Loại Hub: WAREHOUSE / SORTING_CENTER / STATION</summary>
    public string HubType { get; set; } = string.Empty;

    /// <summary>Cấp Hub: 1 = Trung tâm, 2 = Khu vực, 3 = Điểm giao nhận</summary>
    public int HubLevel { get; set; } = 1;

    /// <summary>User Id quản lý Hub này</summary>
    public Guid? ManagerId { get; set; }

    /// <summary>Sức chứa tối đa (số lượng kiện)</summary>
    public int Capacity { get; set; }

    /// <summary>Hub có đang hoạt động không</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Giờ hoạt động (VD: "08:00-20:00")</summary>
    public string? OperatingHours { get; set; }

    /// <summary>Vĩ độ — dùng cho routing/map sau này</summary>
    public double? Latitude { get; set; }

    /// <summary>Kinh độ</summary>
    public double? Longitude { get; set; }

    public ICollection<Rack> Racks { get; set; } = new List<Rack>();
    public ICollection<User> Users { get; set; } = new List<User>();
}