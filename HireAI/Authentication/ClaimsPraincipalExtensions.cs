using System.Security.Claims;

namespace HireAI.Authentication;

public static class ClaimsPraincipalExtensions
{
    public static Guid GetUserID(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId , out Guid parseduserId) ? parseduserId : throw new ApplicationException("User id is unavailable");
    }
}
