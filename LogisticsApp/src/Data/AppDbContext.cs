using LogisticsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsApp.Data;

public class AppDbContext : DbContext
{
    private readonly string _usersTableName;
    private readonly string _itemsTableName;
    private readonly string _driversTableName;
    private readonly string _ordersTableName;
    private readonly string _orderItemsTableName;
    private readonly string _deliveriesTableName;
    private readonly string _locationsTableName;

    public DbSet<User> Users { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<Location> Locations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _usersTableName = configuration["Tables:UsersTableName"] ?? throw new ArgumentNullException("UsersTableName is not provided in configuration.");
        _itemsTableName = configuration["Tables:ItemsTableName"] ?? throw new ArgumentNullException("ItemsTableName is not provided in configuration.");
        _driversTableName = configuration["Tables:DriversTableName"] ?? throw new ArgumentNullException("DriversTableName is not provided in configuration.");
        _ordersTableName = configuration["Tables:OrdersTableName"] ?? throw new ArgumentNullException("OrdersTableName is not provided in configuration.");
        _orderItemsTableName = configuration["Tables:OrderItemsTableName"] ?? throw new ArgumentNullException("OrderItemsTableName is not provided in configuration.");
        _deliveriesTableName = configuration["Tables:DeliveriesTableName"] ?? throw new ArgumentNullException("DeliveriesTableName is not provided in configuration.");
        _locationsTableName = configuration["Tables:LocationsTableName"] ?? throw new ArgumentNullException("LocationsTableName is not provided in configuration.");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Role).IsRequired().HasMaxLength(10);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Phone).IsRequired().HasMaxLength(15);
            entity.Property(u => u.CreatedAt).IsRequired();
            entity.Property(u => u.UpdatedAt).IsRequired(false);

            // Driver-specific fields (nullable for Admins)
            entity.Property(u => u.CurrentPostalCode).IsRequired(false);
            entity.Property(u => u.Status).HasMaxLength(10).HasDefaultValue(DriverStatus.Available);
        });

        // Configure Item entity
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Name).IsRequired().HasMaxLength(100);
            entity.Property(i => i.Description).HasMaxLength(255);
            entity.Property(i => i.Weight).IsRequired();
            entity.Property(i => i.Value).IsRequired();
        });

        // Configure Driver entity (inherits from User)
        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasBaseType<User>();
        });

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);

            // Ensure that each order is part of one delivery (non-nullable)
            entity.HasOne(o => o.Delivery)
                .WithMany(d => d.Orders)  // One delivery can have many orders
                .HasForeignKey(o => o.DeliveryId) // Foreign key for delivery
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete when a delivery is deleted

            entity.HasOne(o => o.Origin)
                .WithMany()
                .HasForeignKey(o => o.OriginId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Destination)
                .WithMany()
                .HasForeignKey(o => o.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        // Configure OrderItem entity
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.Quantity).IsRequired();

            // Set up foreign keys
            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete when an order is deleted

            entity.HasOne(oi => oi.Item)
                .WithMany()
                .HasForeignKey(oi => oi.ItemId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deleting item if there are order items
        });


        // Configure Delivery entity
        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Scheduled");

            // Foreign Key relationships
            entity.HasOne(d => d.Driver)
                .WithMany() // A driver can have many deliveries
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.Orders)  // One delivery can have many orders
                .WithOne(o => o.Delivery)   // Each order is linked to exactly one delivery
                .HasForeignKey(o => o.DeliveryId) // Foreign key on Order
                .OnDelete(DeleteBehavior.Cascade); // Deletes orders when the associated delivery is deleted

            entity.HasOne(d => d.OriginLocation)
                .WithMany()
                .HasForeignKey(d => d.Origin)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.DestinationLocation)
                .WithMany()
                .HasForeignKey(d => d.Destination)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(d => d.DriverId)
                .HasDatabaseName("idx_deliveries_driverid");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            // Ensure that each order has a valid DeliveryId
            entity.Property(o => o.DeliveryId)
                .IsRequired();
        });


        // Configure Location entity
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Name).HasMaxLength(255);
            entity.Property(l => l.City).IsRequired().HasMaxLength(100);
            entity.Property(l => l.State).IsRequired().HasMaxLength(50);
            entity.Property(l => l.PostalCode).IsRequired().HasMaxLength(20);
            entity.Property(l => l.Country).IsRequired().HasMaxLength(100);
            entity.Property(l => l.Latitude).HasColumnType("decimal(9,6)");
            entity.Property(l => l.Longitude).HasColumnType("decimal(9,6)");
        });

        // Table name configuration from settings
        modelBuilder.Entity<User>().ToTable(_usersTableName);
        modelBuilder.Entity<Item>().ToTable(_itemsTableName);
        modelBuilder.Entity<Order>().ToTable(_ordersTableName);
        modelBuilder.Entity<OrderItem>().ToTable(_orderItemsTableName);
        modelBuilder.Entity<Delivery>().ToTable(_deliveriesTableName);
        modelBuilder.Entity<Location>().ToTable(_locationsTableName);
    }
}
