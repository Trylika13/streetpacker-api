using Microsoft.EntityFrameworkCore;
using SP.Domain.Entities;

namespace SP.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    
    public DbSet<User> Users => Set<User>();
    
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    
    public DbSet<Spot> Spots => Set<Spot>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Ad> Ads => Set<Ad>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
    }
}
