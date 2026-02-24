using System;
using System.Collections.Generic;
using System.Text;

namespace ValenceHub.Domain.Common.Exceptions;

public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
