namespace LWMS.Domain.ValueObjects;

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;

    // 💣 THÊM CÁI NÀY (QUAN TRỌNG)
    public Address() { }

    public Address(string street, string province, string district, string ward)
    {
        Street = street;
        Province = province;
        District = district;
        Ward = ward;
    }
}