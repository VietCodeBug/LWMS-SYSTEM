namespace LWMS.Domain.Services;
public class WeightCalculator
{
    /// <summary>
    /// Tính trọng lượng quy đổi từ kích thước (Dimensional Weight).
    /// Công thức: (Dài x Rộng x Cao) / Chỉ số quy đổi (thường là 5000).
    /// Giúp tính toán chi phí cho các kiện hàng cồng kềnh nhưng nhẹ.
    /// </summary>
    public static decimal CalculateDimensionalWeight(decimal length, decimal width, decimal height)
    {
       // 5000 là Dim Factor tiêu chuẩn cho vận chuyển đường hàng không (Air Freight)
        return (length * width *height)/5000;
    }
    /// <summary>
    /// Xác định trọng lượng tính cước (Chargeable Weight).
    /// Theo luật ngành vận tải: Trọng lượng tính tiền là con số lớn nhất 
    /// giữa cân nặng thực tế và cân nặng quy đổi.
    /// </summary>
    public static decimal GetChargeableWeight(decimal actual, decimal dimensional)
    {
        // Nếu hàng nặng thì lấy cân thực, nếu hàng cồng kềnh thì lấy cân quy đổi
        return Math.Max(actual, dimensional);
    }
}