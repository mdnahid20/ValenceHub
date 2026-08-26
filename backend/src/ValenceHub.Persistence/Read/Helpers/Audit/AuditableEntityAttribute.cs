using System;

namespace ValenceHub.Persistence.Read.Helpers.Audit;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AuditableEntityAttribute : Attribute
{
}
