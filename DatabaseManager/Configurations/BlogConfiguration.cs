using BlogLibrairy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseManager.Configurations;

public class BlogConfiguration : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(e => e.Summary)
            .HasMaxLength(1000);
            
        builder.Property(e => e.Content)
            .IsRequired();
            
        builder.Property(e => e.ImageUrl)
            .HasMaxLength(1000);
            
        builder.Property(e => e.CreatedAt);

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