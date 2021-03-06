using System.Security.Claims;
using System.Security.Principal;

namespace GaiaProject.Endpoint.Authentication
{
	public class ActiveUser : ClaimsPrincipal
	{
		public string Auth0Id { get; set; }
		public string Id { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }

		public ActiveUser(IPrincipal principal, string auth0Id, string userId, string username, string email) : base(principal)
		{
			Auth0Id = auth0Id;
			Id = userId;
			Username = username;
			Email = email;
		}
	}
}
