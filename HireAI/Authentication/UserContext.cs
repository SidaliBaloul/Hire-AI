namespace HireAI.Authentication;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{

    public Guid UserId()
    {
        return httpContextAccessor.HttpContext?.User.GetUserID()
        ?? throw new UserContextUnavailableException();
    }
}
