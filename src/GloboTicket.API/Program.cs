using GloboTicket.API.Endpoints;
using GloboTicket.API.Middleware;
using GloboTicket.Application.Interfaces;
using GloboTicket.Infrastructure.Data;
using GloboTicket.Infrastructure.MultiTenancy;
using GloboTicket.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext with SQL Server and NetTopologySuite for spatial types
builder.Services.AddDbContext<GloboTicketDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.UseNetTopologySuite()));

// Register tenant services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<IActService, ActService>();

// Register rate limiting service
builder.Services.AddSingleton<GloboTicket.API.Services.IRateLimitService, GloboTicket.API.Services.RateLimitService>();

// Configure cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".GloboTicket.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.LoginPath = "/auth/login";
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Configure CORS for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GloboTicket API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors();

// Use authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Use custom tenant resolution middleware (after authentication)
app.UseMiddleware<TenantResolutionMiddleware>();

// Use rate limiting middleware (after authentication, before endpoints)
app.UseMiddleware<RateLimitingMiddleware>();

// Map health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck");

// Map authentication endpoints
app.MapAuthEndpoints();

// Map tenant endpoints
app.MapTenantEndpoints();

// Map venue endpoints
app.MapVenueEndpoints();

// Map act endpoints
app.MapActEndpoints();

// Map geocoding endpoints
app.MapGeocodingEndpoints();

app.Run();
