using AuthLibrary;
using DatabaseManager.Configurations.Auth;
using Microsoft.EntityFrameworkCore;

namespace DatabaseManager.DbContexts;

/// <summary>
/// Database context for authentication entities only.
/// Used by the Website with websiteDatabase, separate from content entities.
/// </summary>
public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    // Authentication entities
    public DbSet<LocalAccount> LocalAccounts { get; set; }
    public DbSet<SocialAccount> SocialAccounts { get; set; }
    public DbSet<LocalSession> LocalSessions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<LocalAccountRole> LocalAccountRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply authentication configurations
        modelBuilder.ApplyConfiguration(new LocalAccountConfiguration());
        modelBuilder.ApplyConfiguration(new SocialAccountConfiguration());
        modelBuilder.ApplyConfiguration(new LocalSessionConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new LocalAccountRoleConfiguration());
    }
}
