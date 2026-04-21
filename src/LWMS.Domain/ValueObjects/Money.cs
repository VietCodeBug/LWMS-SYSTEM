namespace LWMS.Domain.ValueObjects;

public class Money
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";

    // 💣 bắt buộc cho EF
    public Money() { }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
}