using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValenceHub.Persistence.Read.Models.Integration;

namespace ValenceHub.Persistence.Read.Configurations.Integration;

public sealed class ProcessedEventRecordConfiguration : IEntityTypeConfiguration<ProcessedEventRecord>
{
    public void Configure(EntityTypeBuilder<ProcessedEventRecord> builder)
    {
        builder.ToTable("ProcessedEvents");

        builder.HasKey(x => x.EventId);

        builder.Property(x => x.EventType)
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(x => x.ProcessedOnUtc)
            .IsRequired();
    }
}
