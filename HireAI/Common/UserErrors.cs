using System.Net.NetworkInformation;

namespace HireAI.Common;

public static class UserErrors
{
    public static readonly Error EmailNotUnique = Error.Conflict("Email Not Unique", "The email address is already in use.");
    public static readonly Error EmailNotFound = Error.NotFound("Email Not Found", "The Entered Email Has Not been Found! ");
    public static Error Unauthorized() => Error.Failure("Users.Unauthorized", "you are unauthorized to perform this action");

    public static readonly Error InvalidRefreshToken = Error.Problem("Users.InvalidRefreshToken", "The provided refresh token is invalid or has expired");
}
