using System;

namespace LWMS.Application.Common.Exceptions;

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}
