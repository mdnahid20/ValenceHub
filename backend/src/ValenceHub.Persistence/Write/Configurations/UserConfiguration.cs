using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValenceHub.Domain.Common.ValueObjects;
using ValenceHub.Domain.Users;

namespace ValenceHub.Persistence.Write.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => UserId.FromGuid(value))
            .ValueGeneratedNever();

        builder.Property(x => x.Email)
            .HasMaxLength(320)
            .HasConversion(
                value => value == null ? null : value.Value,
                value => string.IsNullOrWhiteSpace(value) ? null : Email.Restore(value));

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL");

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(30)
            .HasConversion(
                value => value == null ? null : value.Value,
                value => string.IsNullOrWhiteSpace(value) ? null : PhoneNumber.Restore(value));

        builder.HasIndex(x => x.PhoneNumber)
            .IsUnique()
            .HasFilter("[PhoneNumber] IS NOT NULL");

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.IsVerified)
            .IsRequired();

        builder.Property(x => x.VerifiedAt);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.CreatedBy)
            .HasConversion(
                value => value == null ? (Guid?)null : value.Value,
                value => value.HasValue ? UserId.FromGuid(value.Value) : null);

        builder.Property(x => x.UpdatedBy)
            .HasConversion(
                value => value == null ? (Guid?)null : value.Value,
                value => value.HasValue ? UserId.FromGuid(value.Value) : null);

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.Property(x => x.DeletedAt);

        builder.Property(x => x.DeletedBy)
            .HasConversion(
                value => value == null ? (Guid?)null : value.Value,
                value => value.HasValue ? UserId.FromGuid(value.Value) : null);
    }
}
