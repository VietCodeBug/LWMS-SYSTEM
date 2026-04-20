namespace LWMS.Domain.ValueObjects;
public class Address
{
    public string Street {get;}
    public string Province {get;}
    public string District {get;}
    public string Ward {get;}
    public Address(string street, string province, string district, string ward)
    {
        Street = street;
        Province = province;
        District = district;
        Ward = ward;
    }
}