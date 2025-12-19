using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NanoDMSBusinessService.Data;
using NanoDMSBusinessService.Repositories;
using NanoDMSSharedLibrary;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Or Serilog

builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IBusinessLocationRepository, BusinessLocationRepository>();
builder.Services.AddScoped<IBusinessUserRepository, BusinessUserRepository>();
builder.Services.AddScoped<IBusinessLocationUserRepository, BusinessLocationUserRepository>();
builder.Services.AddScoped<IBusinessConfigRepository, BusinessConfigRepository>();



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
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        // IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        // Update the following line to handle potential null values for "Jwt:Key" in the configuration.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key configuration is missing")))
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

// Global exception middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

app.MapControllers();

// Map areas if needed
app.MapRazorPages();

app.Run();