using System;
using System.Linq;
using System.Reflection;

class Program
{
    static void Main()
    {
        var asm = Assembly.LoadFrom(@"C:\Users\Ian_Pham\.nuget\packages\mysql.entityframeworkcore\10.0.1\lib\net10.0\MySql.EntityFrameworkCore.dll");
        foreach(var t in asm.GetTypes().Where(t => t.Name.Contains("Extension")))
        {
            Console.WriteLine(t.FullName);
            foreach(var m in t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if(m.Name.Contains("Use"))
                    Console.WriteLine(" - " + m.Name);
            }
        }
    }
}
