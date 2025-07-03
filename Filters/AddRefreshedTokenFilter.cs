using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace API.Filters
{
    public class AddRefreshedTokenFilter : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _table;
        private readonly string _idParam;

        public AddRefreshedTokenFilter(string table, string idParam = "profileId")
        {
            _table = table;
            _idParam = idParam;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
            {
                return;
            }

            var db = context.HttpContext.RequestServices.GetService<DatabaseCalls>();
            var user = context.HttpContext.User;

            if (db == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var profileId = user.FindFirst(CustomClaimTypes.UserProfileId)?.Value;
            if (string.IsNullOrEmpty(profileId))
            {
                context.Result = new BadRequestObjectResult("User profile ID is missing.");
                return;
            }

            var routeValues = context.RouteData.Values;
            if (!routeValues.TryGetValue(_idParam, out var idObj) || !int.TryParse(idObj.ToString(), out var resourceId))
            {
                context.Result = new BadRequestObjectResult($"Missing or invalid '{_idParam}' parameter.");
                return;
            }

            var record = await db.GetFromTableAsync(_table, resourceId.ToString());
            if (record.Rows.Count == 0)
            {
                context.Result = new NotFoundObjectResult($"Record with ID {resourceId} not found.");
                return;
            }

            var ownerProfileId = record.Rows[0]["UserProfileId"]?.ToString();
            if (!string.Equals(ownerProfileId, profileId, StringComparison.Ordinal))

            {
                context.Result = new ForbidResult();
            }
        }
    }

}
