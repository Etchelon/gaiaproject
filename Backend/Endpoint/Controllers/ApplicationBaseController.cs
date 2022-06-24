using GaiaProject.Endpoint.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GaiaProject.Endpoint.Controllers
{
	public abstract class ApplicationBaseController : ControllerBase
	{
		protected new ActiveUser? User => HttpContext.User as ActiveUser;
		protected string? UserId => this.User?.Id;
		protected string? Username => this.User?.Username;
	}
}
