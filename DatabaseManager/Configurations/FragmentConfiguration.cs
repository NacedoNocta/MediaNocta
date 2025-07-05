using FragmentLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseManager.Configurations
{
    public class FragmentConfiguration : IEntityTypeConfiguration<Fragment>
    {
        public void Configure(EntityTypeBuilder<Fragment> builder)
        {
            builder.HasKey(f => f.Id);
            
            builder.Property(f => f.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.Content)
                .IsRequired();

            builder.Property(f => f.Summary)
                .IsRequired();

            builder.Property(f => f.TypeTag)
                .HasMaxLength(50);

            builder.Property(f => f.AttachmentType)
                .HasMaxLength(20);

            builder.Property(f => f.VibeCount)
                .HasDefaultValue(0);

            builder.Property(f => f.IsPublic)
                .HasDefaultValue(true);

            builder.Property(f => f.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(f => f.CreatedAt)
                .HasDefaultValueSql("NOW()");

            // Configure Links as JSON
            builder.Property(f => f.Links)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>()
                );

            // Configure Meta as JSON
            builder.Property(f => f.Meta)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null)
                );

            // Configure Tags as JSON
            builder.Property(f => f.Tags)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null)
                );
        }
    }
}