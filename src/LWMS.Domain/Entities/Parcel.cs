using LWMS.Domain.Common;
using LWMS.Domain.Enums;
using LWMS.Domain.ValueObjects;
using LWMS.Domain.Services;

namespace LWMS.Domain.Entities;

/// <summary>
/// 🔥 PARCEL — Entity trung tâm hệ thống LWMS.
/// Mọi luồng nghiệp vụ đều xoay quanh vòng đời của Parcel.
/// </summary>
public class Parcel : BaseEntity, IMustHaveMerchant
{
    // ═══════════════════════════════════════════
    // 📍 IDENTIFICATION
    // ═══════════════════════════════════════════

    /// <summary>Mã vận đơn duy nhất — format: [HUB_CODE][YYYYMMDD][SEQ]</summary>
    public string TrackingCode { get; set; } = string.Empty;

    /// <summary>Merchant sở hữu bưu kiện (multi-tenant key)</summary>
    public Guid MerchantId { get; set; }

    /// <summary>Loại dịch vụ (Standard/Express/Hỏa tốc)</summary>
    public Guid ServiceTypeId { get; set; }

    // ═══════════════════════════════════════════
    // 🏗️ HUB FLOW — Theo dõi hành trình qua các Hub
    // ═══════════════════════════════════════════

    /// <summary>Hub lấy hàng ban đầu (nơi merchant gửi)</summary>
    public Guid OriginHubId { get; set; }

    /// <summary>Hub đích cuối cùng (gần receiver nhất)</summary>
    public Guid DestHubId { get; set; }

    /// <summary>Hub hiện tại đang giữ bưu kiện (null = đang trên đường)</summary>
    public Guid? CurrentHubId { get; set; }

    // ═══════════════════════════════════════════
    // 👤 SENDER / RECEIVER — Thông tin liên hệ
    // ═══════════════════════════════════════════

    public string SenderName { get; set; } = string.Empty;
    public string SenderPhone { get; set; } = string.Empty;
    public Address SenderAddress { get; set; } = null!;

    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public Address ReceiverAddress { get; set; } = null!;

    // ═══════════════════════════════════════════
    // ⚖️ WEIGHT & DIMENSION
    // ═══════════════════════════════════════════

    /// <summary>Cân nặng thực tế (gram)</summary>
    public decimal Weight { get; set; }

    /// <summary>Chiều dài (cm) — dùng tính dimensional weight</summary>
    public decimal Length { get; set; }

    /// <summary>Chiều rộng (cm)</summary>
    public decimal Width { get; set; }

    /// <summary>Chiều cao (cm)</summary>
    public decimal Height { get; set; }

    // ═══════════════════════════════════════════
    // 💰 FINANCIAL BREAKDOWN — Chi tiết tài chính
    // ═══════════════════════════════════════════

    /// <summary>Tiền thu hộ (COD) — Value Object Money</summary>
    public Money CodAmount { get; set; } = null!;

    /// <summary>Phí vận chuyển</summary>
    public decimal ShippingFee { get; set; }

    /// <summary>Phí bảo hiểm</summary>
    public decimal InsuranceFee { get; set; }

    /// <summary>Tổng phí</summary>
    public decimal TotalFee { get; set; }

    /// <summary>Giảm giá (nếu có)</summary>
    public decimal Discount { get; set; }

    /// <summary>Người trả phí: SENDER hoặc RECEIVER</summary>
    public string PaidBy { get; set; } = "SENDER";

    // ═══════════════════════════════════════════
    // 📋 STATUS & LIFECYCLE
    // ═══════════════════════════════════════════

    /// <summary>Trạng thái hiện tại (18 trạng thái — xem ParcelStatus enum)</summary>
    public ParcelStatus Status { get; set; } = ParcelStatus.Created;

    /// <summary>Số lần giao hàng (mỗi lần shipper cố giao = +1)</summary>
    public int DeliveryAttempts { get; set; } = 0;

    /// <summary>Lý do giao thất bại gần nhất</summary>
    public string? FailReason { get; set; }

    /// <summary>Ngày cam kết giao (SLA)</summary>
    public DateTime? SlaDate { get; set; }

    // ═══════════════════════════════════════════
    // 🚩 OPERATIONAL FLAGS
    // ═══════════════════════════════════════════

    /// <summary>Hàng dễ vỡ — cần xử lý đặc biệt</summary>
    public bool IsFragile { get; set; }

    /// <summary>Hàng giá trị cao — cần bảo hiểm</summary>
    public bool IsHighValue { get; set; }

    /// <summary>Đây là đơn hoàn trả (đang trên đường về merchant)</summary>
    public bool IsReturn { get; set; }

    /// <summary>Mô tả hàng hóa bên trong</summary>
    public string? Description { get; set; }

    /// <summary>Ghi chú nội bộ (chỉ staff/admin thấy)</summary>
    public string? Notes { get; set; }

    // ═══════════════════════════════════════════
    // 🔗 NAVIGATION PROPERTIES
    // ═══════════════════════════════════════════
    public ICollection<TrackingLog> TrackingLogs { get; set; } = new List<TrackingLog>();
    public ICollection<BagItem> BagItems { get; set; } = new List<BagItem>();
    public ICollection<ShipperAssignment> ShipperAssignments { get; set; } = new List<ShipperAssignment>();
    public ICollection<CodRecord> CodRecords { get; set; } = new List<CodRecord>();
    public ICollection<ParcelLocation> ParcelLocations { get; set; } = new List<ParcelLocation>();
    public ReturnOrder? ReturnOrder { get; set; }

    // ═══════════════════════════════════════════
    // 🧠 DOMAIN METHODS
    // ═══════════════════════════════════════════

    /// <summary>
    /// Chuyển trạng thái bưu kiện (validate theo State Machine).
    /// </summary>
    public void ChangeStatus(ParcelStatus newStatus)
    {
        if (!ParcelStateMachine.CanTransition(Status, newStatus))
        {
            throw new InvalidOperationException(
                $"Quy trình không hợp lệ: Không thể chuyển từ {Status} sang {newStatus}. Vui lòng kiểm tra lại luồng nghiệp vụ."
            );
        }
        Status = newStatus;
    }

    /// <summary>
    /// Chuyển trạng thái + tạo TrackingLog (append-only).
    /// </summary>
    public TrackingLog ChangeStatusWithLog(ParcelStatus newStatus, Guid actorId, string location)
    {
        if (!ParcelStateMachine.CanTransition(Status, newStatus))
        {
            throw new InvalidOperationException($"Vi phạm State Machine: Từ {Status} không được phép nhảy sang {newStatus}.");
        }
        
        var log = new TrackingLog
        {
            ParcelId = this.Id,
            FromStatus = this.Status,
            ToStatus = newStatus,
            ActorId = actorId,
            Location = location,
            CreatedTime = DateTime.UtcNow
        };
        
        Status = newStatus;
        return log;
    }
}