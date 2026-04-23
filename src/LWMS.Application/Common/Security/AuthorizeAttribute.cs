using System;

namespace LWMS.Application.Common.Security;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeAttribute : Attribute
{
    public AuthorizeAttribute() { }

    public string Roles { get; set; } = string.Empty;
}
