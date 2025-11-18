using Microsoft.EntityFrameworkCore;
using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Infrastructure.Data;

public class AppDbContext: DbContext
{
    public DbSet<Email>  Emails { get; set; }
    public DbSet<CredentialsDb> Credentials { get; set; }
    public DbSet<Logs> Logs { get; set; }
    public DbSet<InstanceDB> InstanceDBs { get; set; }
    public DbSet<User>  Users { get; set; }
    public DbSet<Role>  Roles { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Role - User 1 - M
        modelBuilder.Entity<Role>()
            .HasMany(u => u.Users)
            .WithOne(r => r.Role)
            .HasForeignKey(r => r.RoleId);
        
        // User - Email 1 - M
        modelBuilder.Entity<User>()
            .HasMany(u => u.Emails)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.UserId);
        
        // User -Instance 1 - M
        modelBuilder.Entity<User>()
            .HasMany(u => u.InstancesDB)
            .WithOne(i => i.User)
            .HasForeignKey(i => i.UserId);
        
        // Instance - Logs 1 - M
        modelBuilder.Entity<Logs>()
            .HasOne(i => i.InstanceDB)
            .WithMany(l => l.Logs)
            .HasForeignKey(i => i.InstanceId);
        
        // Eamil - Credentials 1 - 1
        modelBuilder.Entity<Email>()
            .HasOne(e => e.CredentialsDB)
            .WithOne(g => g.Email)
            .HasForeignKey<CredentialsDb>(g => g.EmailId);
        
   
    }
    
}