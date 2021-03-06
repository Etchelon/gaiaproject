using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaiaProject.Endpoint.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]/[action]")]
	[Produces("application/json")]
	public abstract class ApiBaseController : ApplicationBaseController
	{
	}
}
