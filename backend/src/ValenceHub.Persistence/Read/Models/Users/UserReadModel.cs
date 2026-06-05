using System;
using System.Collections.Generic;
using System.Text;
using ValenceHub.Persistence.Read.Helpers.Audit;

namespace ValenceHub.Persistence.Read.Models.Users;

[AuditableEntity]
public sealed class UserReadModel
{
    [IgnoreAudit]
    public Guid Id { get; set; }
    [IgnoreAudit]
    public Guid EventId { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsVerified { get; set; }
}
