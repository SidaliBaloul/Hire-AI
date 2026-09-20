using HireAI.Common;
using HireAI.Features.Candidates;

namespace HireAI.Features.Users;

public class UserItem : Entity
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public List<Candidate> Cadidates { get; set; } = [];

}
