using AuthLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseManager.Configurations.Auth;

public class LocalSessionConfiguration : IEntityTypeConfiguration<LocalSession>
{
    public void Configure(EntityTypeBuilder<LocalSession> builder)
    {
        builder.HasKey(e => e.Id);

        // Encrypted tokens stored as strings
        builder.Property(e => e.AccessToken)
            .IsRequired();

        builder.Property(e => e.RefreshToken)
            .IsRequired();

        builder.Property(e => e.IdToken)
            .IsRequired();

        // Token expiration metadata
        builder.Property(e => e.AccessTokenExpiration)
            .IsRequired();

        builder.Property(e => e.RefreshTokenExpiration)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastAccessedAt)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Unique constraint on LocalAccountId (one session per user)
        builder.HasIndex(e => e.LocalAccountId)
            .IsUnique();
    }
}
