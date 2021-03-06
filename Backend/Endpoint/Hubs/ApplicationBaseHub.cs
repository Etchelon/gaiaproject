using GaiaProject.Endpoint.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GaiaProject.Endpoint.Hubs
{
	[Authorize]
	public class ApplicationBaseHub : Hub
	{
		protected ActiveUser User => Context.User as ActiveUser;
		protected string UserId => this.User?.Id;
		protected string Username => this.User?.Username;
	}
}