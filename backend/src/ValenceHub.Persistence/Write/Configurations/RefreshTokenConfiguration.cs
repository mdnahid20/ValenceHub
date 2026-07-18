using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValenceHub.Persistence.Write.Models.Auth;

namespace ValenceHub.Persistence.Write.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<PersistedRefreshToken>
{
    public void Configure(EntityTypeBuilder<PersistedRefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.RevokedAtUtc);
    }
}
