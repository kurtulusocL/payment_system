using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentSystem.Infrastructure.Data.Context.Azure;
using PaymentSystem.Infrastructure.Data.Context.Local.Mssql;

namespace PaymentSystem.Infrastructure.Constants.Workers
{
    public class AzureSyncWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AzureSyncWorker> _logger;

        public AzureSyncWorker(IServiceProvider serviceProvider, ILogger<AzureSyncWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AzureSyncWorker: Waiting 30s for SQL Server...");
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var localContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var azureContext = scope.ServiceProvider.GetRequiredService<AzureDbContext>();

                    if (!await localContext.Database.CanConnectAsync(stoppingToken))
                    {
                        _logger.LogWarning("DB not ready, retrying in 20s...");
                        await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                        continue;
                    }

                    var pendingEvents = await localContext.OutboxEvents
                        .Where(e => !e.IsProcessed)
                        .OrderBy(e => e.CreatedDate)
                        .Take(50)
                        .ToListAsync(stoppingToken);

                    if (!pendingEvents.Any())
                    {
                        await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                        continue;
                    }

                    _logger.LogInformation("{Count} Outbox events to sync", pendingEvents.Count);

                    foreach (var ev in pendingEvents)
                    {
                        try
                        {
                            if (ev.EntityType == "AspNetUserLogins")
                            {
                                var loginData = JsonSerializer.Deserialize<Dictionary<string, string>>(ev.Payload);
                                if (loginData == null) { ev.IsProcessed = true; continue; }

                                var userLogin = new Microsoft.AspNetCore.Identity.IdentityUserLogin<string>
                                {
                                    LoginProvider = loginData["LoginProvider"],
                                    ProviderKey = loginData["ProviderKey"],
                                    ProviderDisplayName = loginData.GetValueOrDefault("ProviderDisplayName"),
                                    UserId = loginData["UserId"]
                                };

                                var exists = await azureContext.Set<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>()
                                    .AnyAsync(l => l.LoginProvider == userLogin.LoginProvider && l.ProviderKey == userLogin.ProviderKey, stoppingToken);

                                if (!exists)
                                {
                                    await azureContext.Set<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().AddAsync(userLogin, stoppingToken);
                                    await azureContext.SaveChangesAsync(stoppingToken);
                                }

                                ev.IsProcessed = true;
                                continue;
                            }

                            if (ev.EntityType == "AspNetUserRoles")
                            {
                                var roleData = JsonSerializer.Deserialize<Dictionary<string, string>>(ev.Payload);
                                if (roleData == null) { ev.IsProcessed = true; continue; }

                                var userRole = new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
                                {
                                    UserId = roleData["UserId"],
                                    RoleId = roleData["RoleId"]
                                };

                                var rExists = await azureContext.Set<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>()
                                    .AnyAsync(r => r.UserId == userRole.UserId && r.RoleId == userRole.RoleId, stoppingToken);

                                if (!rExists)
                                {
                                    await azureContext.Set<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().AddAsync(userRole, stoppingToken);
                                    await azureContext.SaveChangesAsync(stoppingToken);
                                }

                                ev.IsProcessed = true;
                                continue;
                            }

                            var entityType = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes())
                                .FirstOrDefault(t => t.Name == ev.EntityType && t.Namespace == "PaymentSystem.Domain.Entities");

                            if (entityType == null)
                            {
                                _logger.LogWarning("Type not found: {EntityType}", ev.EntityType);
                                ev.IsProcessed = true;
                                continue;
                            }

                            var entity = JsonSerializer.Deserialize(ev.Payload, entityType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                            if (entity != null)
                            {
                                var id = entityType.GetProperty("Id")?.GetValue(entity);

                                if (ev.EventType == "Deleted")
                                {
                                    var existing = await azureContext.FindAsync(entityType, id);
                                    if (existing != null)
                                    {
                                        azureContext.Remove(existing);
                                        await azureContext.SaveChangesAsync(stoppingToken);
                                    }
                                }
                                else
                                {
                                    var existing = await azureContext.FindAsync(entityType, id);
                                    if (existing != null)
                                        azureContext.Entry(existing).CurrentValues.SetValues(entity);
                                    else
                                        await azureContext.AddAsync(entity, stoppingToken);

                                    await azureContext.SaveChangesAsync(stoppingToken);
                                }

                                ev.IsProcessed = true;
                                _logger.LogInformation("Synced: {EntityType} ID: {EventId}", ev.EntityType, ev.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error syncing {EntityType}", ev.EntityType);
                            azureContext.ChangeTracker.Clear();
                        }
                    }

                    await localContext.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Outbox table updated");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "AzureSyncWorker error, retrying in 20s...");
                }

                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }
    }
}
