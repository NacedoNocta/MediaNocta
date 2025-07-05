using FragmentLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseManager.Configurations
{
    public class FragmentTypeConfiguration : IEntityTypeConfiguration<FragmentType>
    {
        public void Configure(EntityTypeBuilder<FragmentType> builder)
        {
            builder.HasKey(ft => ft.Id);
            
            builder.Property(ft => ft.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ft => ft.Color)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(ft => ft.CreatedAt)
                .HasDefaultValueSql("NOW()");

            builder.HasIndex(ft => ft.Name)
                .IsUnique();
        }
    }
}