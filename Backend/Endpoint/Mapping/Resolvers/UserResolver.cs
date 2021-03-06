using AutoMapper;
using GaiaProject.ViewModels.Users;
using GaiaProject.Engine.DataAccess.Abstractions;

namespace GaiaProject.Endpoint.Mapping.Resolvers
{
	public class UserResolver : IMemberValueResolver<object, object, string, UserViewModel>
	{
		private readonly IProvideData _dataProvider;

		public UserResolver(IProvideData dataProvider)
		{
			_dataProvider = dataProvider;
		}

		public UserViewModel Resolve(object source, object destination, string userId, UserViewModel destMember, ResolutionContext context)
		{
			var user = _dataProvider.GetUser(userId).Result;
			return context.Mapper.Map<UserViewModel>(user);
		}
	}
}
