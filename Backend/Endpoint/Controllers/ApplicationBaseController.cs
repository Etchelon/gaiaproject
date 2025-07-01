using GaiaProject.Endpoint.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GaiaProject.Endpoint.Controllers
{
	public abstract class ApplicationBaseController : ControllerBase
	{
		protected new ActiveUser? User => HttpContext.User as ActiveUser;
		protected string? UserId => User?.Id.ToString();
		protected string? Username => User?.Username;
	}
}
