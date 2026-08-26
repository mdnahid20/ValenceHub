using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValenceHub.Persistence.Read.Models.Users;

namespace ValenceHub.Persistence.Read.Configurations.Users;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityName)
            .HasMaxLength(200);

        builder.Property(x => x.Action)
            .HasConversion<short>()
            .IsRequired();

        var comparer = new ValueComparer<Dictionary<string, object?>?>(
            (left, right) => Serialize(left) == Serialize(right),
            value => Serialize(value).GetHashCode(),
            value => Deserialize(Serialize(value)));

        builder.Property(x => x.ChangedData)
            .HasConversion(
                value => Serialize(value),
                value => Deserialize(value))
            .Metadata.SetValueComparer(comparer);
    }

    private static string Serialize(Dictionary<string, object?>? data)
        => JsonSerializer.Serialize(data ?? new Dictionary<string, object?>(), JsonOptions);

    private static Dictionary<string, object?> Deserialize(string? json)
        => string.IsNullOrWhiteSpace(json)
            ? new Dictionary<string, object?>()
            : JsonSerializer.Deserialize<Dictionary<string, object?>>(json, JsonOptions)
                ?? new Dictionary<string, object?>();
}
