using System.Security.Claims;
using System.Security.Principal;

namespace GaiaProject.Endpoint.Authentication;

public class ActiveUser : ClaimsPrincipal
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }

    public ActiveUser(IPrincipal principal, string userId, string username, string email) : base(principal)
    {
        Id = userId;
        Username = username;
        Email = email;
    }
}
