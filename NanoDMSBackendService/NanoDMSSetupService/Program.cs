using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NanoDMSSetupService.Data;
using NanoDMSSetupService.Repositories;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Serial log

builder.Host.UseSerilog((context, services, logger) =>
{
    logger
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("ServiceName", "dms-setup-service") // CHANGE PER SERVICE
        .WriteTo.Console()
        .WriteTo.Seq("http://207.180.234.174:5341");
});

// =======================
// Standard Health Checks
// =======================

var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(defaultConnectionString))
{
    throw new InvalidOperationException("DefaultConnection connection string is missing or null.");
}

builder.Services.AddHealthChecks()
    .AddNpgSql(
        defaultConnectionString,
        name: "PostgreSQL",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy
    );

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis:6379";
    options.InstanceName = "DmsCache_";
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IGenderRepository, GenderRepository>();
builder.Services.AddScoped<IMaritalRepository, MaritalRepository>();
builder.Services.AddScoped<ITimeZoneRepository, TimeZoneRepository>();
//builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IFinancialYearRepository, FinancialYearRepository>();
builder.Services.AddScoped<IStockAccountingRepository, StockAccountingRepository>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true; // Ensure the email is unique
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // Enforce valid characters for email/username

    // Lockout Settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30); // 30 minutes lockout
    options.Lockout.MaxFailedAccessAttempts = 5; // Max 5 failed login attempts before lockout
    options.Lockout.AllowedForNewUsers = true; // Apply for all new users as well

    // Password settings
    options.Password.RequiredLength = 12; // Minimum password length
    options.Password.RequireDigit = true; // Requires at least one numeric character
    options.Password.RequireLowercase = true; // Requires at least one lowercase character
    options.Password.RequireUppercase = true; // Requires at least one uppercase character
    options.Password.RequireNonAlphanumeric = true; // Requires at least one special character
    options.Password.RequiredUniqueChars = 1; // Requires unique characters
    options.Password.RequiredLength = 12;// FI it 
})
// Store identity data in your database
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(1); // Tokens are valid for 1 hour
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new InvalidOperationException("Jwt:Key configuration is missing or null.");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddRazorPages();


// Add services to the container.
builder.Services.AddControllersWithViews(); // Add MVC support for controllers with views

builder.Services.AddAuthorization();

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

// Logs every HTTP request automatically
app.UseSerilogRequestLogging();

// Map health endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new {
                name = e.Key,
                status = e.Value.Status.ToString(),
                exception = e.Value.Exception?.Message,
                duration = e.Value.Duration.TotalMilliseconds
            })
        };

        await context.Response.WriteAsJsonAsync(result);
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("AllowAll"); // Apply the CORS policy to allow all origins
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map areas if needed
app.MapRazorPages();

app.Run();
