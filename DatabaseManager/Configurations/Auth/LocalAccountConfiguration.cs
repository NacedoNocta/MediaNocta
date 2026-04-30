using AuthLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseManager.Configurations.Auth;

public class LocalAccountConfiguration : IEntityTypeConfiguration<LocalAccount>
{
    public void Configure(EntityTypeBuilder<LocalAccount> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Username)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(e => e.Username)
            .IsUnique();

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // One-to-one relationship with SocialAccount
        builder.HasOne(e => e.SocialAccount)
            .WithOne(e => e.LocalAccount)
            .HasForeignKey<SocialAccount>(e => e.LocalAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-one relationship with LocalSession (one session per user)
        builder.HasOne(e => e.LocalSession)
            .WithOne(e => e.LocalAccount)
            .HasForeignKey<LocalSession>(e => e.LocalAccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
