using System;
using LWMS.Application.Parcels.Commands.Create;
public static class Program
{
    public static void Main()
    {
        var cmd = new CreateParcelCommand { SenderName = "A" };
        Console.WriteLine(cmd.SenderName);
    }
}
