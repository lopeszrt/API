using API.Services;

namespace API.Middleware
{
    public class JwtRefreshMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtRefreshMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, JwtService jwtService)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var refreshedToken = jwtService.RefreshToken(context);
                context.Items["RefreshedToken"] = refreshedToken;
            }

            await _next(context);
        }
    }

}
