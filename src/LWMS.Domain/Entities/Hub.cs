using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;
public class Hub : BaseEntity
{
    public string HubCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ProvinceCode { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    public int Capacity { get; set; }
}