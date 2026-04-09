using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using PaymentSystem.Application.Authorization.Attributes;
using PaymentSystem.Application.Authorization.Requirements;

namespace PaymentSystem.Application.Authorization.Handlers
{
    public class ProfileOwnerRequirementHandler : AuthorizationHandler<ProfileOwnerRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public ProfileOwnerRequirementHandler(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfileOwnerRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            var endpoint = httpContext.GetEndpoint();
            var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (actionDescriptor != null)
            {
                var skipCheck = actionDescriptor.MethodInfo.GetCustomAttribute<SkipOwnershipCheckAttribute>();
                if (skipCheck != null)
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                context.Fail();
                return;
            }

            var routeId = httpContext.Request.RouteValues["id"]?.ToString();
            if (string.IsNullOrWhiteSpace(routeId))
                routeId = httpContext.Request.Query["id"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(routeId))
            {
                context.Succeed(requirement);
                return;
            }

            var controller = httpContext.Request.RouteValues["controller"]?.ToString()?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(controller))
            {
                context.Fail();
                return;
            }

            bool isAllowedController = controller == "payment" ||
                                       controller == "wallet" ||
                                       controller == "transaction" ||
                                       controller == "merchant" ||
                                       controller == "user" ||
                                       controller == "usersession";
            if (!isAllowedController)
            {
                context.Fail();
                return;
            }

            if (currentUserId.Equals(routeId, StringComparison.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var action = httpContext.Request.RouteValues["action"]?.ToString()?.ToLowerInvariant();
            var entity = await FindEntityAsync(httpContext, scope, routeId, action);

            if (entity == null)
            {
                context.Fail();
                return;
            }

            var ownerProp = entity.GetType().GetProperty("UserId") ??
                            entity.GetType().GetProperty("Id");

            if (ownerProp == null)
            {
                context.Fail();
                return;
            }

            var ownerValue = ownerProp.GetValue(entity)?.ToString();
            if (string.Equals(ownerValue, currentUserId, StringComparison.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }

        private async Task<object> FindEntityAsync(HttpContext httpContext, IServiceScope scope, string entityId, string actionLower)
        {
            var endpoint = httpContext.GetEndpoint();
            var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (actionDescriptor == null) return null;

            var serviceTypeAttribute = actionDescriptor.MethodInfo.GetCustomAttribute<ServiceTypeAttribute>();
            if (serviceTypeAttribute == null) return null;

            var service = scope.ServiceProvider.GetService(serviceTypeAttribute.ServiceType);
            if (service == null) return null;

            var getByIdMethod = service.GetType().GetMethod("GetByIdAsync");
            if (getByIdMethod == null) return null;

            try
            {
                var task = (Task)getByIdMethod.Invoke(service, new object[] { entityId });
                await task.ConfigureAwait(false);
                return task.GetType().GetProperty("Result")?.GetValue(task);
            }
            catch
            {
                return null;
            }
        }
    }
}
