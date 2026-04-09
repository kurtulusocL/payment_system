using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.Repositories.Abstract;

namespace PaymentSystem.Infrastructure.Constants.Attributes
{
    public class AuditLogAttribute : ActionFilterAttribute
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var area = context.ActionDescriptor.DisplayName;

            var resultContext = await next();

            if (resultContext.Exception == null)
            {
                var auditRepository = resultContext.HttpContext.RequestServices.GetRequiredService<IAuditRepository>();
                var outboxRepository = resultContext.HttpContext.RequestServices.GetRequiredService<IOutboxEventRepository>();

                var auditLog = new Audit
                {
                    AppUserId = userId,
                    AreaAccessed = area,
                    CreatedDate = DateTime.Now
                };
                await auditRepository.AddAsync(auditLog);

                var outboxEvents = new OutboxEvent
                {
                    EntityType = "Audit",
                    EventType = "Added",
                    Payload = JsonSerializer.Serialize(auditLog, new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    }),
                    CreatedDate = DateTime.Now,
                    IsProcessed = false,
                    IsActive = true,
                    IsDeleted = false
                };
                await outboxRepository.AddAsync(outboxEvents);
                await SaveToFileAsJsonAsync(auditLog, resultContext.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().WebRootPath);
            }
        }
        private async Task SaveToFileAsJsonAsync(Audit audit, string webRootPatht)
        {
            var logFolder = Path.Combine(webRootPatht, "Logs");
            Directory.CreateDirectory(logFolder);
            var filePath = Path.Combine(logFolder, "AuditLogs.txt");

            var jsonString = JsonSerializer.Serialize(audit, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            });

            await _semaphore.WaitAsync();
            try
            {
                await File.AppendAllTextAsync(filePath, jsonString + Environment.NewLine);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"File writing error: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
