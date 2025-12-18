using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot and configuration support
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.Configure<FormOptions>(options =>
{

    options.MultipartBodyLengthLimit = long.MaxValue; // or a reasonable large value like 1_000_000_000
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue; // Allow large payloads
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
});


// Add Ocelot services
builder.Services.AddOcelot();

builder.Services.AddControllers();

// Configure CORS to allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

// Root of the application (works on any machine)
var imagesRootPath = Path.Combine(app.Environment.ContentRootPath, "Images");

if (!Directory.Exists(imagesRootPath))
{
    Directory.CreateDirectory(imagesRootPath);
}

// Serve images from /Images
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesRootPath),
    RequestPath = "/Images"
});


// Use Ocelot Middleware
app.UseHttpsRedirection();
app.UseCors("AllowAll"); // Apply the CORS policy to allow all origins
app.UseAuthorization();

app.MapControllers();

await app.UseOcelot(); // Ensure this is before `app.Run()`

app.Run();
