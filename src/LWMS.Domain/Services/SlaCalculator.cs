using LWMS.Domain.Entities;

namespace LWMS.Domain.Services;
public class SlaCalculator
{
    public static DateTime Calculate(
        string fromProvince,
        string toProvince,
        ServiceType serviceType,
        DateTime createdAt
    )
    {
        int days =serviceType.MaxDays;
        //Cung tinh SLA dựa trên tỉnh gửi, tỉnh nhận và loại dịch vụ
        if (fromProvince == toProvince)
        {
          days =1;
        }
        //Ví dụ: nếu là dịch vụ Express thì nhanh hơn, nếu là dịch vụ Standard thì chậm hơn
        if (serviceType.IsExpress())
        {
            days -= 1;
        }
       var result = createdAt.AddDays(days);
      // 🔴 bỏ chủ nhật
        if (result.DayOfWeek == DayOfWeek.Sunday)
            result = result.AddDays(1);

        return result;
    }
}