using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;
public class ServiceType :BaseEntity
{
    public string Code {get;set;} = string.Empty;
    public string Name {get;set;} = string.Empty;
    public int MaxDays {get;set;} // Số ngày tối đa để hoàn thành đơn hàng
    public decimal BaseFee {get;set;} // Phí cơ bản cho dịch vụ
    public string EstimatedDays { get; set; } = string.Empty; // Mô tả thời gian (VD: 3-5 ngày)
    public bool IsActive {get;set;} = true; // Trạng thái hoạt động của dịch vụ
    public bool IsExpress()
    {
        return Code.ToUpper() == "EXPRESS";
    }
}