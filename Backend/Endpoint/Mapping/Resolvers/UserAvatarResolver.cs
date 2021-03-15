using AutoMapper;
using GaiaProject.Core.Logic;
using GaiaProject.Engine.DataAccess.Abstractions;

namespace GaiaProject.Endpoint.Mapping.Resolvers
{
	public class UserAvatarResolver : IMemberValueResolver<object, object, string, string>
	{
		private readonly UserManager _userManager;

		public UserAvatarResolver(UserManager userManager)
		{
			_userManager = userManager;
		}

		public string Resolve(object source, object destination, string id, string destMember, ResolutionContext context)
		{
			var user = _userManager.GetUser(id).Result;
			return string.IsNullOrEmpty(user.Avatar) ? "https://lorempixel.com/150/150" : user.Avatar;
		}
	}
}
