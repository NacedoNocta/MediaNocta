using TechLibrary;
using SharedLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace DatabaseManager.Configurations;

public class TechUpdateConfiguration : IEntityTypeConfiguration<TechUpdate>
{
    public void Configure(EntityTypeBuilder<TechUpdate> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Configure LocalizedText properties as JSON
        builder.Property(e => e.Title)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<LocalizedText>(v, (JsonSerializerOptions?)null) ?? new LocalizedText())
            .IsRequired()
            .HasColumnType("jsonb");
            
        builder.Property(e => e.Summary)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<LocalizedText>(v, (JsonSerializerOptions?)null) ?? new LocalizedText())
            .HasColumnType("jsonb");
            
        builder.Property(e => e.Content)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<LocalizedText>(v, (JsonSerializerOptions?)null) ?? new LocalizedText())
            .IsRequired()
            .HasColumnType("jsonb");
            
        builder.Property(e => e.ImageUrl)
            .HasMaxLength(1000);
            
        builder.Property(e => e.CreatedAt);

        // Configure TechUpdate-specific properties
        builder.Property(e => e.ProjectId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.UpdateType)
            .HasMaxLength(100);

        // Configure inherited Activity properties
        builder.Property(e => e.Featured);

        // Ignore computed properties
        builder.Ignore(e => e.ActivityType);
        builder.Ignore(e => e.ThemeColor);

        // Configure Links as converted property
        builder.Property(e => e.Links)
            .HasConversion(
                v => v != null ? string.Join(";", v) : null,
                v => v != null ? v.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>());

        // Add index on ProjectId for performance
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.CreatedAt);
    }
}