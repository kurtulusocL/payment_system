using System.Collections.Concurrent;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentSystem.Infrastructure.Data.Context.Local.Mssql;

namespace PaymentSystem.Infrastructure.Constants.Extensions
{
    public static class SecurityExtensions
    {
        private static readonly ConcurrentDictionary<string, DateTime> BannedIps = new();

        private static DateTime _lastCleanup = DateTime.UtcNow;
        private const int CLEANUP_INTERVAL_MINUTES = 30;

        private static HashSet<string> _staticExtensions = new(StringComparer.OrdinalIgnoreCase);
        private static HashSet<string> _blockedAgents = new(StringComparer.OrdinalIgnoreCase);

        private static DateTime _lastSettingsUpdate = DateTime.MinValue;
        private static int _settingsUpdateInProgress = 0;
        private const int SETTINGS_CACHE_MINUTES = 5;

        public static IServiceCollection AddCustomSecurity(this IServiceCollection services, IConfiguration config)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = 429;

                options.AddFixedWindowLimiter("LoginPolicy", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueLimit = 0;
                    opt.AutoReplenishment = true;
                });

                options.AddFixedWindowLimiter("SignalRPolicy", opt =>
                {
                    opt.PermitLimit = config.GetValue<int>("RateLimit:SignalR:PermitLimit");
                    opt.Window = TimeSpan.FromSeconds(config.GetValue<int>("RateLimit:SignalR:WindowSeconds"));
                    opt.QueueLimit = 5;
                    opt.AutoReplenishment = true;
                });

                options.AddFixedWindowLimiter("WebPolicy", opt =>
                {
                    opt.PermitLimit = config.GetValue<int>("RateLimit:Web:PermitLimit");
                    opt.Window = TimeSpan.FromSeconds(config.GetValue<int>("RateLimit:Web:WindowSeconds"));
                    opt.QueueLimit = 0;
                    opt.AutoReplenishment = true;
                });

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var ip = GetIpAddress(context) ?? "unknown";
                    var path = context.Request.Path.ToString().ToLower();
                    var isSignalR = path.Contains("/hub") || path.Contains("/signalr");

                    if (isSignalR)
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: $"signalr:{ip}",
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = config.GetValue<int>("RateLimit:SignalR:PermitLimit"),
                                Window = TimeSpan.FromSeconds(config.GetValue<int>("RateLimit:SignalR:WindowSeconds")),
                                QueueLimit = 5,
                                AutoReplenishment = true
                            });
                    }
                    else if (path.Contains("/auth/login") && context.Request.Method == "POST")
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: $"login:{ip}",
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 5,
                                Window = TimeSpan.FromMinutes(1),
                                QueueLimit = 0,
                                AutoReplenishment = true
                            });
                    }
                    else
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: $"web:{ip}",
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = config.GetValue<int>("RateLimit:Web:PermitLimit"),
                                Window = TimeSpan.FromSeconds(config.GetValue<int>("RateLimit:Web:WindowSeconds")),
                                QueueLimit = 0,
                                AutoReplenishment = true
                            });
                    }
                });

                options.OnRejected = async (context, token) =>
                {
                    var ip = GetIpAddress(context.HttpContext) ?? "unknown";
                    var path = context.HttpContext.Request.Path;

                    if (!BannedIps.ContainsKey(ip))
                        BannedIps.TryAdd(ip, DateTime.UtcNow.AddHours(24));

                    CleanupExpiredBans();

                    context.HttpContext.Response.StatusCode = 429;
                    context.HttpContext.Response.ContentType = "application/json";

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        error = "Too many requests",
                        message = "You sent too many requests. You have been banned for 24 hours.",
                        retryAfter = 86400
                    }, cancellationToken: token);
                };
            });

            services.AddSingleton(BannedIps);
            return services;
        }

        public static IApplicationBuilder UseCustomSecurity(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
                await UpdateSettingsCacheIfNeeded(dbContext);

                var ext = Path.GetExtension(context.Request.Path.Value ?? string.Empty).ToLowerInvariant();
                if (_staticExtensions.Contains(ext))
                {
                    await next();
                    return;
                }

                var bannedIps = context.RequestServices.GetRequiredService<ConcurrentDictionary<string, DateTime>>();
                var ip = GetIpAddress(context);

                if (ip != null && bannedIps.TryGetValue(ip, out var banEnd))
                {
                    if (DateTime.UtcNow < banEnd)
                    {
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync("Your IP is banned for 24 hours.");
                        return;
                    }
                    bannedIps.TryRemove(ip, out _);
                }

                var ua = context.Request.Headers.UserAgent.ToString();
                if (_blockedAgents.Any(bot => ua.Contains(bot, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Forbidden.");
                    return;
                }

                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var sessionIp = context.Session.GetString("OriginalIP");
                    var sessionUa = context.Session.GetString("OriginalUA");

                    if (sessionIp != null && (sessionIp != ip || sessionUa != ua))
                    {
                        await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(context);
                        context.Session.Clear();
                        context.Response.Redirect("/auth/login?error=security_breach");
                        return;
                    }
                }

                await next();
            });

            app.UseRateLimiter();
            return app;
        }

        private static async Task UpdateSettingsCacheIfNeeded(ApplicationDbContext dbContext)
        {
            if (DateTime.UtcNow.Subtract(_lastSettingsUpdate).TotalMinutes < SETTINGS_CACHE_MINUTES)
                return;

            if (Interlocked.CompareExchange(ref _settingsUpdateInProgress, 1, 0) != 0)
                return;

            try
            {
                if (DateTime.UtcNow.Subtract(_lastSettingsUpdate).TotalMinutes < SETTINGS_CACHE_MINUTES)
                    return;

                var settings = await dbContext.SecuritySettings
                    .Where(s => s.IsActive && !s.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync();

                _staticExtensions = new HashSet<string>(
                    settings.Where(s => s.Type == "StaticExtension").Select(s => s.Value),
                    StringComparer.OrdinalIgnoreCase);

                _blockedAgents = new HashSet<string>(
                    settings.Where(s => s.Type == "BlockedAgent").Select(s => s.Value),
                    StringComparer.OrdinalIgnoreCase);

                _lastSettingsUpdate = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SecuritySettings cache update failed: {ex.Message}");
            }
            finally
            {
                Interlocked.Exchange(ref _settingsUpdateInProgress, 0);
            }
        }

        private static string? GetIpAddress(HttpContext context)
        {
            var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwarded))
                return forwarded.Split(',')[0].Trim();

            return context.Connection.RemoteIpAddress?.ToString();
        }

        private static void CleanupExpiredBans()
        {
            if (DateTime.UtcNow.Subtract(_lastCleanup).TotalMinutes < CLEANUP_INTERVAL_MINUTES)
                return;

            var expired = BannedIps.Where(x => x.Value < DateTime.UtcNow).ToList();
            foreach (var entry in expired)
                BannedIps.TryRemove(entry.Key, out _);

            _lastCleanup = DateTime.UtcNow;
        }
    }
}
