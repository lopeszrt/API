using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;


namespace API.Filters
{
    public class CheckUserIdFilter: IAsyncActionFilter
    {
        private readonly JwtService _jwtService;

        public CheckUserIdFilter(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
            {
                return;
            }

            var db = context.HttpContext.RequestServices.GetRequiredService<DatabaseCalls>();
            var user = context.HttpContext.User;
            if (user.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId) || userId <= 0)
            {
                context.Result = new BadRequestObjectResult(new { error = "Invalid User ID." });
                return;
            }

            var routeValues = context.RouteData.Values;
            if (routeValues.TryGetValue("id", out var idValue) && idValue is string idStr && int.TryParse(idStr, out var id) && id > 0)
            {
                // Check if the user has access to the resource with the given ID
                var hasAccess = await db.CheckUserAccessAsync(userId, id);
                if (!hasAccess)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
            else
            {
                // If no ID is provided, just continue
                await next();
                return;
            }
        }
    }
}
