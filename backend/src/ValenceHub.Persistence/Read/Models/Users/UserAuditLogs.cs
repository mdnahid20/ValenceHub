using System;
using System.Collections.Generic;
using System.Text;
using ValenceHub.Domain.Common.Enums;

namespace ValenceHub.Persistence.Read.Models.Users;

public sealed class AuditLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ActionBy { get; set; }
    public Guid EventId { get; set; }
    public Guid EntityId { get; set; }  
    public string? EntityName { get; set; } 
    public EntityAction Action { get; set; }
    public DateTimeOffset OccurredOnUtc { get; set; }
    public Dictionary<string, object?>? ChangedData { get; set; }

}
