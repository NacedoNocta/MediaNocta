using BlogLibrairy;
using Microsoft.EntityFrameworkCore;

namespace DatabaseManager
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Tag> Tags { get; set; }
    }
}
