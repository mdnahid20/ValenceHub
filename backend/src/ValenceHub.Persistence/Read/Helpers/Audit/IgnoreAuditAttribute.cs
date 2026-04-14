using System;
using System.Collections.Generic;
using System.Text;

namespace ValenceHub.Persistence.Read.Helpers.Audit;

[AttributeUsage(AttributeTargets.Property)]
public sealed class IgnoreAuditAttribute : Attribute
{
}