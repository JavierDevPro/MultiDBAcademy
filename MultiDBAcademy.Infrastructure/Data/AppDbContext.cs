using Microsoft.EntityFrameworkCore;
using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Email> Emails { get; set; }
    public DbSet<CredentialsDb> Credentials { get; set; }
    public DbSet<Logs> Logs { get; set; }
    public DbSet<InstanceDB> InstanceDBs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Claves primarias
        modelBuilder.Entity<Role>().HasKey(r => r.Id);
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<CredentialsDb>().HasKey(c => c.Id);
        modelBuilder.Entity<Email>().HasKey(e => e.Id);
        modelBuilder.Entity<Logs>().HasKey(l => l.Id);
        modelBuilder.Entity<InstanceDB>().HasKey(i => i.Id);

        // Role - User (1 - M)
        modelBuilder.Entity<Role>()
            .HasMany(u => u.Users)
            .WithOne(r => r.Role)
            .HasForeignKey(r => r.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // User - Email (1 - M)
        modelBuilder.Entity<User>()
            .HasMany(u => u.Emails)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // User - InstanceDB (1 - M)
        modelBuilder.Entity<User>()
            .HasMany(u => u.InstancesDB)
            .WithOne(i => i.User)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // InstanceDB - Logs (1 - M)
        modelBuilder.Entity<InstanceDB>()
            .HasMany(i => i.Logs)
            .WithOne(l => l.InstanceDB)
            .HasForeignKey(l => l.InstanceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // InstanceDB - CredentialsDb (1 - 1)
        modelBuilder.Entity<InstanceDB>()
            .HasOne(i => i.Credentials)
            .WithOne(c => c.Instance)
            .HasForeignKey<InstanceDB>(i => i.CredentialsDbId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Email - CredentialsDb (1 - 1)
        modelBuilder.Entity<Email>()
            .HasOne(e => e.CredentialsDB)
            .WithOne(c => c.Email)
            .HasForeignKey<Email>(e => e.CredentialsDBId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Índices para mejorar rendimiento
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        modelBuilder.Entity<InstanceDB>()
            .HasIndex(i => new { i.DatabaseName, i.EngineType })
            .IsUnique();
    }
}