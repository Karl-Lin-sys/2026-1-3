using Microsoft.EntityFrameworkCore;
using SmartBuildingIoT.Data;
using SmartBuildingIoT.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=smartbuilding.db"));

// AI Services
builder.Services.AddSingleton<AnomalyDetector>();
builder.Services.AddSingleton<SensorSimulator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();
app.UseStaticFiles();
app.MapControllers();

// Default route
app.MapGet("/", () => Results.Redirect("/index.html"));

Console.WriteLine("===========================================");
Console.WriteLine("  SmartBuildingIoT Dashboard");
Console.WriteLine("  http://localhost:5000");
Console.WriteLine("===========================================");

app.Run("http://localhost:5000");
