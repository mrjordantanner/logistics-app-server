using Microsoft.EntityFrameworkCore;
using LogisticsApp.Contexts;
using LogisticsApp.Repositories;
using LogisticsApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
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
builder.Services.AddControllers();

// Configure services
ConfigureServices(builder.Services, builder.Configuration);

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS policy globally
app.UseCors("AllowLocalhost4200");

//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Helper method to configure additional services
void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Add EF Core with SQL Server
    var connectionString = configuration.GetConnectionString("AzureSQLDatabase");
    services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));

    // Add custom services and repositories with Scoped lifetime
    services.AddScoped<IItemRepository, ItemRepository>();
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IUserService, UserService>();

    // Add Swagger for API documentation
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddHttpClient();
}
