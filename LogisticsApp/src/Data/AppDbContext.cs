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
        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            // Use the Role column as the discriminator
            entity.HasDiscriminator<string>("Role")
                .HasValue<Admin>("Admin")  // Admin role maps to Admin class
                .HasValue<Driver>("Driver");  // Driver role maps to Driver class

            // Properties from the User class
            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.Phone)
                .HasMaxLength(15);  // Optional phone number field, can be null

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.PasswordSalt)
                .IsRequired();

            entity.Property(u => u.Role)
                .IsRequired()
                .HasDefaultValue("Driver");  // Default to 'Driver' role as string

            entity.Property(u => u.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            entity.Property(u => u.UpdatedAt)
                .HasColumnType("datetime2");

            // Driver-specific fields (nullable for Admins)
            entity.Property(u => u.CurrentPostalCode);

            // Driver-specific Status (needs to be string in the database)
            entity.Property(u => u.Status)
                .HasConversion<string>()  // Convert DriverStatus enum to string in the database
                .HasDefaultValue(DriverStatus.Available);
        });


        // Driver
        modelBuilder.Entity<Driver>(entity =>
        {
            // Explicitly specify that Driver is a subclass of User
            entity.HasBaseType<User>();  // This makes sure Driver is treated as a subclass of User

            // Driver-specific properties, overriding the base User class
            entity.Property(d => d.CurrentPostalCode)
                .HasMaxLength(10);  // You can define a length for postal code if needed

            entity.Property(d => d.Status)
                .HasConversion<string>()  // Ensure the DriverStatus enum is stored as string
                .HasDefaultValue(DriverStatus.Available);
        });


        // Item
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Name).IsRequired().HasMaxLength(100);
            entity.Property(i => i.Description).HasMaxLength(255);
            entity.Property(i => i.Weight).IsRequired();
            entity.Property(i => i.Value).IsRequired();
        });


        // Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);

            //entity.Property(o => o.DeliveryId);

            //entity.Property(u => u.CreatedAt)
            //    .IsRequired()
            //    .HasColumnType("datetime2");

            //entity.Property(u => u.UpdatedAt)
            //    .HasColumnType("datetime2");

            entity.Property(o => o.OrderStatus)
                .HasDefaultValue(OrderStatus.Pending)
                .HasConversion<string>();

            //entity.HasOne(o => o.Delivery)
            //    .WithMany(d => d.Orders)  // One delivery can have many orders
            //    .HasForeignKey(o => o.DeliveryId)
            //    .OnDelete(DeleteBehavior.Cascade); // Cascade delete when a delivery is deleted

            entity.HasOne(o => o.Origin)
                .WithMany()
                .HasForeignKey(o => o.OriginId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Destination)
                .WithMany()
                .HasForeignKey(o => o.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        // OrderItem
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


        // Delivery
        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue(DeliveryStatus.Scheduled)
                .HasConversion<string>();

            // Foreign Key relationships
            entity.HasOne(d => d.Driver)
                .WithMany() // A driver can have many deliveries
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            //entity.HasMany(d => d.Orders)  // One delivery can have many orders
            //    .WithOne(o => o.Delivery)   // Each order is linked to exactly one delivery
            //    .HasForeignKey(o => o.DeliveryId) // Foreign key on Order
            //    .OnDelete(DeleteBehavior.Cascade); // Deletes orders when the associated delivery is deleted

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


        // Location
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
