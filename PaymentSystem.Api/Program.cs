using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using PaymentSystem.Application.Authorization.Handlers;
using PaymentSystem.Application.Authorization.Requirements;
using PaymentSystem.Application.Constants.Helpers;
using PaymentSystem.Application.Filters;
using PaymentSystem.Application.Hubs;
using PaymentSystem.Application.Mapping;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.Constants.Attributes;
using PaymentSystem.Infrastructure.Constants.Extensions;
using PaymentSystem.Infrastructure.Constants.Handlers;
using PaymentSystem.Infrastructure.Constants.Options;
using PaymentSystem.Infrastructure.DependencyResolver.DependencyInjection;
using PaymentSystem.Infrastructure.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Logs", "SerilogLogs.txt");
Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().WriteTo
    .File(logPath, rollingInterval: RollingInterval.Infinite, rollOnFileSizeLimit: true, fileSizeLimitBytes: 10 * 1024 * 1024,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}").CreateLogger();
builder.Host.UseSerilog();

builder.Services.DependencyService(builder.Configuration, builder.Environment);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidatorActionFilter>();
    options.Filters.Add<ExceptionHandlerAttribute>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddScoped<IAuthorizationHandler, ProfileOwnerRequirementHandler>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, AdditionalUserClaimsPrincipalFactory>();
builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddAuthentication().AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", null);
builder.Services.AddMemoryCache();
builder.Services.AddMappingProfiles();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});
builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSecurity(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProfileOwner", policy =>
        policy.Requirements.Add(new ProfileOwnerRequirement()));

    options.AddPolicy("AdminsOnly", policy =>
        policy.RequireRole("Admins"));

    options.AddPolicy("SecondAdminOnly", policy =>
        policy.RequireRole("SecondAdmins"));

    options.AddPolicy("HelperAdminsOnly", policy =>
        policy.RequireRole("HelperAdmins"));

    options.AddPolicy("UsersOnly", policy =>
        policy.RequireRole("Users"));

    options.AddPolicy("AdminsAndSecondAdmin", policy =>
        policy.RequireRole("Admins", "SecondAdmins"));

    options.AddPolicy("AllAdmins", policy =>
        policy.RequireRole("Admins", "SecondAdmins", "HelperAdmins"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("PaymentSystemCorsPolicy", policy =>
        policy.WithOrigins("https://localhost:5002")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());

    //if you crearte desktop app
    options.AddPolicy("AllowDesktopApp", policy =>
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());

    options.AddPolicy("SignalRPolicy", policy =>
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PaymentSystem API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header: Bearer {token}"
    });
    c.OperationFilter<AuthorizeOperationFilter>();
});

var app = builder.Build();

ServiceProviderHelper.ServiceProvider = app.Services;
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentSystem API V1");
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseSerilogRequestLogging();
app.UseRateLimiter();
app.UseRouting();
app.UseCustomSecurity();
app.UseCors("AllowDesktopApp");
app.UseCors("PaymentSystemCorsPolicy");
app.UseCors("SignalRPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.MapGet("/", (HttpContext context) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger"))
        return Results.Ok();
    
    var apiInfo = new
    {
        name = "PaymentSystem API",
        version = "v1",
        description = "A Modular Payment Processing Backend",
        status = "running",
        timestamp = DateTime.UtcNow,
        endpoints = new
        {
            health = "/health",
            swagger = "/swagger",
            signalR = "/hubs/{payment,wallet,transaction}"
        },
        documentation = "Use /swagger for API documentation and testing"
    };

    return Results.Ok(apiInfo);
});

app.MapHub<PaymentHub>("/hubs/payment");
app.MapHub<WalletHub>("/hubs/wallet");
app.MapHub<TransactionHub>("/hubs/transaction");

app.MapControllers();
app.Run();

Log.CloseAndFlush();
