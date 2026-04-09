using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PaymentSystem.Infrastructure.Identity
{
    public abstract class AppControllerBase : ControllerBase
    {
        protected string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        protected IActionResult Unauthorized(string message)
        {
            return StatusCode(401, message);
        }

        protected IActionResult Forbidden(string message)
        {
            return StatusCode(403, message);
        }

        protected static async Task<IActionResult> CheckUserOwnershipAsync(
            HttpContext httpContext,
            string entityId,
            Func<string, Task<object>> getEntityById,
            Func<object, string> getUserIdFromEntity)
        {
            if (string.IsNullOrWhiteSpace(entityId))
                return new BadRequestObjectResult("Id is mandatory.");

            var entity = await getEntityById(entityId);
            if (entity == null)
                return new NotFoundResult();

            // JWT claim'den al — session değil, IDOR koruması burada
            var currentUserId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
                return new UnauthorizedObjectResult("User auth information is missing or wrong.");

            var entityUserId = getUserIdFromEntity(entity);
            if (string.IsNullOrWhiteSpace(entityUserId) ||
                !entityUserId.Equals(currentUserId, StringComparison.OrdinalIgnoreCase))
            {
                return new ObjectResult("You are not authorized for this request.") { StatusCode = 403 };
            }

            return null;
        }
    }
}
