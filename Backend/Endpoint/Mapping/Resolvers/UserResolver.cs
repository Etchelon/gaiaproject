using AutoMapper;
using GaiaProject.Core.Logic;
using GaiaProject.ViewModels.Users;
using GaiaProject.Engine.DataAccess.Abstractions;

namespace GaiaProject.Endpoint.Mapping.Resolvers
{
	public class UserResolver : IMemberValueResolver<object, object, string, UserViewModel>
	{
		private readonly UserManager _userManager;

		public UserResolver(UserManager userManager)
		{
			_userManager = userManager;
		}

		public UserViewModel Resolve(object source, object destination, string userId, UserViewModel destMember, ResolutionContext context)
		{
			var user = _userManager.GetUser(userId).Result;
			return context.Mapper.Map<UserViewModel>(user);
		}
	}
}
