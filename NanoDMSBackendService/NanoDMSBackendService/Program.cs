using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog setup
builder.Host.UseSerilog((context, services, logger) =>
{
    logger
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("ServiceName", "dms-gateway-service")
        .WriteTo.Console()
        .WriteTo.Seq("http://207.180.234.174:5341");
});

// Health checks
builder.Services.AddHealthChecks();

// Ocelot and configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

// Controllers
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Kestrel config
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
});

var app = builder.Build();

// Serilog request logging
app.UseSerilogRequestLogging();

// Health endpoint — MUST be before Ocelot
app.MapHealthChecks("/health");

var imagesRootPath = "/opt/dms/dms-gateway/Images";

if (!Directory.Exists(imagesRootPath))
    Directory.CreateDirectory(imagesRootPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesRootPath),
    RequestPath = "/Images"
});


// Middleware
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Ocelot middleware LAST
await app.UseOcelot();

app.Run();
