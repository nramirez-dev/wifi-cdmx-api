using Microsoft.EntityFrameworkCore;
using WifiCdmx.Application.Interfaces;
using WifiCdmx.Application.Services;
using WifiCdmx.Infrastructure.Data;
using WifiCdmx.Infrastructure.Repositories;
using WifiCdmx.Infrastructure.Seeders;

var builder = WebApplication.CreateBuilder(args);

// --- Database ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Repositories & Services (Dependency Injection) ---
builder.Services.AddScoped<IWifiPointRepository, WifiPointRepository>();
builder.Services.AddScoped<IWifiPointService, WifiPointService>();
builder.Services.AddScoped<CsvSeeder>();

// --- Controllers ---
builder.Services.AddControllers();

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "WiFi CDMX API",
        Version = "v1",
        Description = "REST API for querying public WiFi access points in Mexico City."
    });

    // Include XML comments in Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// --- Auto migrate and seed on startup ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<CsvSeeder>();
    var csvPath = builder.Configuration["CsvSeeder:FilePath"] ?? "Data/wifi_cdmx.csv";
    await seeder.SeedAsync(csvPath);
}

// --- Middleware ---
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WiFi CDMX API v1"));

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
