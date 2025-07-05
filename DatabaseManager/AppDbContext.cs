using BlogLibrary;
using FragmentLibrary;
using DatabaseManager.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DatabaseManager
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Fragment> Fragments { get; set; }
        public DbSet<FragmentType> FragmentTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new BlogConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new FragmentConfiguration());
            modelBuilder.ApplyConfiguration(new FragmentTypeConfiguration());
        }
    }
}
