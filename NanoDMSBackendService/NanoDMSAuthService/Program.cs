using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NanoDMSAuthService.Data;
using NanoDMSAuthService.Models;
using NanoDMSAuthService.Services;
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


builder.Services.AddTransient<EmailService>();

// Register services
builder.Services.AddHttpClient<ApiServiceHelper>();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
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
    options.TokenLifespan = TimeSpan.FromHours(12); // Tokens are valid for 1 hour
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Fix for CS8604: Ensure the configuration value is not null before using it.
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

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB limit
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 104857600; // 100 MB
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // 100 MB
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

// 1. ADD THIS AT THE VERY TOP OF THE PIPELINE
// This tells .NET to trust the headers from Nginx (X-Forwarded-Proto, etc.)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
// 2. PATH BASE
// Ensure this matches exactly how you call the URL (see Step 2 below)
app.UsePathBase("/apigateway/AuthService");

app.UseRouting();
//app.UseHttpsRedirection();
app.UseCors("AllowAll"); // Apply the CORS policy to allow all origins

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering(); // Allows multiple reads of the request body
    await next();
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map areas if needed
app.MapRazorPages();

app.Run();
