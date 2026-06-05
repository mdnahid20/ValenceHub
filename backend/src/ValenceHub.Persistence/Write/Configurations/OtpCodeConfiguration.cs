using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValenceHub.Domain.Otps;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Persistence.Write.Configurations;

internal sealed class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        builder.ToTable("OtpCodes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => OtpCodeId.FromGuid(value))
            .ValueGeneratedNever();

        builder.Property(x => x.UserId);

        builder.Property(x => x.TargetType)
            .HasConversion<short>()
            .IsRequired();

        builder.Property(x => x.TargetValue)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(x => x.Purpose)
            .HasConversion<short>()
            .IsRequired();

        builder.Property(x => x.CodeHash)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.UsedAtUtc);

        builder.Property(x => x.Attempts)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.ResendCount)
            .IsRequired();

        builder.Property(x => x.LastSentAtUtc)
            .IsRequired();

        builder.Property(x => x.VerificationTokenHash)
            .HasMaxLength(64);

        builder.Property(x => x.VerificationTokenExpiresAtUtc);

        builder.Property(x => x.VerificationTokenConsumedAtUtc);
        //TODO : Later we should work on it.
  //      builder.HasIndex(x => new { x.TargetType, x.TargetValue, x.Purpose, x.CreatedAtUtc });
    }
}
