using HireAI.Features.Users;

namespace HireAI.Authentication;

public interface ITokenProvider
{
    string Create(UserItem user);

    string GenerateRefreshToken();
}
