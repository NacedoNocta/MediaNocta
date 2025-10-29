using AuthLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseManager.Configurations.Auth;

public class SocialAccountConfiguration : IEntityTypeConfiguration<SocialAccount>
{
    public void Configure(EntityTypeBuilder<SocialAccount> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Provider)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.ProviderUserId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.ProviderUsername)
            .HasMaxLength(255);

        builder.Property(e => e.ProviderEmail)
            .HasMaxLength(255);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        // Unique constraint on Provider + ProviderUserId
        builder.HasIndex(e => new { e.Provider, e.ProviderUserId })
            .IsUnique();

        // Unique constraint on LocalAccountId (one-to-one relationship)
        builder.HasIndex(e => e.LocalAccountId)
            .IsUnique();
    }
}
