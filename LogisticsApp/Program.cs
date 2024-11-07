using Microsoft.EntityFrameworkCore;
using LogisticsApp.Contexts;
using LogisticsApp.Repositories;
using LogisticsApp.Services;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();
app.UseAuthorization(); // If you have authentication/authorization
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
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IUserService, UserService>();

    // Add controllers
    services.AddControllers();

    // Add Swagger for API documentation
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddHttpClient();

}
