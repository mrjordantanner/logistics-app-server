using Microsoft.EntityFrameworkCore;
using LogisticsApp.Data;
using LogisticsApp.Services;
using LogisticsApp.Repositories;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder.Services, builder.Configuration);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost4200");
//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Add CORS policy
    services.AddCors(options =>
    {
        options.AddPolicy("AllowLocalhost4200", builder =>
        {
            builder
                .WithOrigins("http://localhost:4200", "https://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    // Add controllers
    services.AddControllers();

    // Add EF Core with SQL Server
    var connectionString = configuration.GetConnectionString("AzureSQLDatabase");
    services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));

    // Add repositories
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IItemRepository, ItemRepository>();
    services.AddScoped<ILocationRepository, LocationRepository>();
    services.AddScoped<IOrderItemRepository, OrderItemRepository>();
    services.AddScoped<IOrderRepository, OrderRepository>();
    services.AddScoped<IDeliveryRepository, DeliveryRepository>();

    // Add services
    services.AddScoped<IDeliveryService, DeliveryService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IOrderService, OrderService>();

    // Add Swagger for API documentation
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddHttpClient();
}
