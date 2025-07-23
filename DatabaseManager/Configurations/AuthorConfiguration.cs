using BlogLibrary;
using SharedLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace DatabaseManager.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(e => e.Biography)
            .HasConversion(
                v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
                v => v != null ? JsonSerializer.Deserialize<LocalizedText>(v, (JsonSerializerOptions?)null) : null)
            .HasColumnType("jsonb");
            
        builder.Property(e => e.ImageUrl)
            .HasMaxLength(1000);

        builder.Property(e => e.Email)
            .HasMaxLength(300);

        builder.Property(e => e.Website)
            .HasMaxLength(500);

        builder.Property(e => e.Twitter)
            .HasMaxLength(100);

        builder.Property(e => e.LinkedIn)
            .HasMaxLength(300);

        builder.Property(e => e.GitHub)
            .HasMaxLength(100);
    }
}