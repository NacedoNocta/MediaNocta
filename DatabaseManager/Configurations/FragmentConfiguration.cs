using FragmentLibrary;
using SharedLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace DatabaseManager.Configurations
{
    public class FragmentConfiguration : IEntityTypeConfiguration<Fragment>
    {
        public void Configure(EntityTypeBuilder<Fragment> builder)
        {
            builder.HasKey(f => f.Id);
            
            // Configure LocalizedText properties as JSON
            builder.Property(f => f.Title)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<LocalizedText>(v, (JsonSerializerOptions?)null) ?? new LocalizedText())
                .IsRequired()
                .HasColumnType("jsonb");

            builder.Property(f => f.Content)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<LocalizedText>(v, (JsonSerializerOptions?)null) ?? new LocalizedText())
                .IsRequired()
                .HasColumnType("jsonb");

            builder.Property(f => f.Summary)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<LocalizedText>(v, (JsonSerializerOptions?)null) ?? new LocalizedText())
                .IsRequired()
                .HasColumnType("jsonb");

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

            // Configure inherited Activity properties
            builder.Property(f => f.Featured);

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