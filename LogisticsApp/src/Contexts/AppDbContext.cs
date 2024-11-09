using LogisticsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsApp.Contexts
{
    public class AppDbContext : DbContext
    {
        private readonly string _userTableName;
        private readonly string _itemTableName;

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _userTableName = configuration["Tables:UserTableName"] ?? throw new ArgumentNullException("UserTableName is not provided in configuration.");
            _itemTableName = configuration["Tables:ItemTableName"] ?? throw new ArgumentNullException("ItemTableName is not provided in configuration.");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Role).IsRequired();
                entity.Property(u => u.CreatedAt).IsRequired();
            });

            // Configure Item entity
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Name).IsRequired().HasMaxLength(100);
                entity.Property(i => i.Description).HasMaxLength(255);
                entity.Property(i => i.Weight).IsRequired();
                entity.Property(i => i.Value).IsRequired();
                entity.Property(i => i.Size).IsRequired();
            });

            // Optional: Table name configuration from settings
            modelBuilder.Entity<User>().ToTable(_userTableName);
            modelBuilder.Entity<Item>().ToTable(_itemTableName);
        }
    }
}
