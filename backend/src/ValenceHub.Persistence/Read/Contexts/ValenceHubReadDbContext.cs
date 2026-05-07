using System;
using Microsoft.EntityFrameworkCore;
using ValenceHub.Persistence.Read.Models.Integration;
using ValenceHub.Persistence.Read.Models.Users;

namespace ValenceHub.Persistence.Read.Contexts;

public class ReadDbContext : DbContext
{
    private const string ConfigurationNamespacePrefix = "ValenceHub.Persistence.Read.Configurations";

    public ReadDbContext(DbContextOptions<ReadDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserReadModel> Users => Set<UserReadModel>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ProcessedEventRecord> ProcessedEvents => Set<ProcessedEventRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ReadDbContext).Assembly,
            IsReadConfigurationType);

        base.OnModelCreating(modelBuilder);
    }

    private static bool IsReadConfigurationType(Type type)
        => type.Namespace?.StartsWith(ConfigurationNamespacePrefix, StringComparison.Ordinal) == true;
}
