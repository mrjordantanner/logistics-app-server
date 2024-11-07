using LogisticsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsApp.Contexts;

public class AppDbContext : DbContext
{
    private readonly string _tableName;
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _tableName = configuration["UserTableName"] ?? throw new ArgumentNullException("Table name is not provided in configuration.");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.Property(u => u.Role).IsRequired();
            entity.Property(u => u.CreatedAt).IsRequired();
        });
    }
}
