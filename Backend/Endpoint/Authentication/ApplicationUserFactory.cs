using GaiaProject.Engine.Logic;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GaiaProject.Endpoint.Authentication
{
	public class ApplicationUserFactory : IClaimsTransformation
	{
		private readonly UserManager _userManager;

		public ApplicationUserFactory(UserManager userManager)
		{
			_userManager = userManager;
		}

		public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
		{
			var auth0Id = principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
			var user = await _userManager.GetUserByIdentifier(auth0Id);
			var applicationUser = new ActiveUser(principal, auth0Id, user?.Id, user?.Username, user?.Email);
			return applicationUser;
		}
	}
}
