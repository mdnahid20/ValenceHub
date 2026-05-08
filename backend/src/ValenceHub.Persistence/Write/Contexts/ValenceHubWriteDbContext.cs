using System;
using Microsoft.EntityFrameworkCore;
using ValenceHub.Domain.Users;
using ValenceHub.Persistence.Write.Outbox.Models;
using ValenceHub.Persistence.Write.Models.Auth;

namespace ValenceHub.Persistence.Write.Contexts;

public class ValenceHubWriteDbContext : DbContext
{
    private const string ConfigurationNamespacePrefix = "ValenceHub.Persistence.Write.Configurations";

    public ValenceHubWriteDbContext(DbContextOptions<ValenceHubWriteDbContext> options)
        : base(options)
    {
    }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<PersistedRefreshToken> RefreshTokens => Set<PersistedRefreshToken>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ValenceHubWriteDbContext).Assembly,
            IsWriteConfigurationType);

        base.OnModelCreating(modelBuilder);
    }

    private static bool IsWriteConfigurationType(Type type)
        => type.Namespace?.StartsWith(ConfigurationNamespacePrefix, StringComparison.Ordinal) == true;
}
