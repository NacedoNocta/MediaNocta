using BlogLibrary;
using SharedLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace DatabaseManager.Configurations;

public class BlogConfiguration : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
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

        // Configure new blog properties
        builder.Property(e => e.ContentType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.State)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.UpdatedAt);
        
        builder.Property(e => e.PublishedAt);

        builder.Property(e => e.Slug)
            .HasMaxLength(500);

        builder.Property(e => e.ReadingTimeMinutes);

        builder.Property(e => e.Views);

        // Configure inherited Activity properties
        builder.Property(e => e.Featured);

        // Ignore computed properties
        builder.Ignore(e => e.ActivityType);
        builder.Ignore(e => e.ThemeColor);

        // Configure relationship with Author
        builder.HasOne(b => b.Author)
            .WithMany()
            .HasForeignKey("AuthorId")
            .OnDelete(DeleteBehavior.Restrict);

        // Configure many-to-many relationship with Tags
        builder.HasMany(b => b.Tags)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "BlogTag",
                b => b.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                t => t.HasOne<Blog>().WithMany().HasForeignKey("BlogId"));

        // Configure Links as converted property
        builder.Property(e => e.Links)
            .HasConversion(
                v => v != null ? string.Join(";", v) : null,
                v => v != null ? v.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>());
    }
}