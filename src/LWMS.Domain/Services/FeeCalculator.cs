namespace LWMS.Domain.Services;
public class FeeCalculator
{
    public static decimal CalculateShippingFee(decimal weight)
    {
        if (weight <= 1000)
        {
            return 15000;
        }
        var extraKg = Math.Ceiling((weight-1000)/1000);
        return 15000 + extraKg * 5000;
    }
    public static decimal CalculateCodFee(decimal codAmount)
    {
        return codAmount * 0.01m; // 1% COD fee
    }
    public static decimal CalculateTotalFee(decimal weght,decimal codAmount)
    {
        var shipping = CalculateShippingFee(weght);
        var codFee = CalculateCodFee(codAmount);
        return shipping + codFee;
    }
}