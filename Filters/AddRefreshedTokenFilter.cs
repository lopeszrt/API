using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    public class AddRefreshedTokenFilter : IAsyncResultFilter
    {

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var httpContext = context.HttpContext;

            if (httpContext.User.Identity?.IsAuthenticated == true &&
                context.Result is ObjectResult { StatusCode: >= 200 and < 300 } objectResult)
            {
                if (httpContext.Items.TryGetValue("RefreshedToken", out var tokenObj) && tokenObj is string refreshedToken)
                {
                    objectResult.Value = new
                    {
                        res = objectResult.Value,
                        token = refreshedToken
                    };
                }
            }

            await next();
        }
    }

}