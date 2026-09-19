using Scalar.AspNetCore;

namespace HireAI.Common.Extensions;

public static class ApplicationBuilderExtension
{
    public static WebApplication MapOpenApiWithScalar(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();

        return app;
    }
}
