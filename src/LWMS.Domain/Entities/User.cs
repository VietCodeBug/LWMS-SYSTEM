
using LWMS.Domain.Common;
using LWMS.Domain.Enums;

namespace LWMS.Domain.Entities;
public class User :BaseEntity
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Guid? HubId { get; set; }

}