using AutoMapper;
using GaiaProject.Engine.Logic;

namespace GaiaProject.Endpoint.Mapping.Resolvers
{
	public class UsernameResolver : IMemberValueResolver<object, object, string, string>
	{
		private readonly UserManager _userManager;

		public UsernameResolver(UserManager userManager)
		{
			_userManager = userManager;
		}

		public string Resolve(object source, object destination, string userId, string destMember, ResolutionContext context)
		{
			var username = _userManager.GetUsername(userId).Result;
			return username;
		}
	}
}
