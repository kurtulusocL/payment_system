using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.Data.Context.Local.Mssql;

namespace PaymentSystem.Infrastructure.Constants.Attributes
{    
    public class ExceptionHandlerAttribute : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            var serviceProvider = filterContext.HttpContext.RequestServices;
            if (serviceProvider == null)
                return;

            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                var logger = new ExceptionLogger
                {
                    ExceptionMessage = filterContext.Exception.Message,
                    ExceptionStackTrace = filterContext.Exception.StackTrace,
                    ControllerName = filterContext.RouteData?.Values["controller"]?.ToString() ?? "Unknown",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                };

                dbContext.ExceptionLoggers.Add(logger);

                dbContext.OutboxEvents.Add(new OutboxEvent
                {
                    EntityType = "ExceptionLogger",
                    EventType = "Added",
                    Payload = JsonSerializer.Serialize(logger),
                    CreatedDate = DateTime.UtcNow,
                    IsProcessed = false,
                    IsActive = true,
                    IsDeleted = false
                });
                dbContext.SaveChanges();
                filterContext.ExceptionHandled = true;
            }
            catch
            {
                filterContext.ExceptionHandled = true;
            }
        }
    }
}
