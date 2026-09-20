using HireAI.Common.Middleware;

namespace HireAI.Common.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        app.UseMiddleware<SecurityHeadersMiddleware>();

        return app;
    }
}
