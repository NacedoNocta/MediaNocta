using AuthLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseManager.Configurations.Auth;

public class LocalAccountRoleConfiguration : IEntityTypeConfiguration<LocalAccountRole>
{
    public void Configure(EntityTypeBuilder<LocalAccountRole> builder)
    {
        // Composite primary key
        builder.HasKey(e => new { e.LocalAccountId, e.RoleId });

        builder.Property(e => e.AssignedAt)
            .IsRequired();

        // Many-to-many relationship with LocalAccount
        builder.HasOne(e => e.LocalAccount)
            .WithMany(e => e.LocalAccountRoles)
            .HasForeignKey(e => e.LocalAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-many relationship with Role
        builder.HasOne(e => e.Role)
            .WithMany(e => e.LocalAccountRoles)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
