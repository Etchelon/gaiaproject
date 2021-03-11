using System.Threading.Tasks;
using AutoMapper;
using GaiaProject.Core.Logic;
using GaiaProject.Core.Model;
using GaiaProject.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GaiaProject.Endpoint.Controllers
{
	public class UsersController : ApiBaseController
	{
		private readonly ILogger<UsersController> _logger;
		private readonly UserManager _userManager;
		private readonly IMapper _mapper;

		public UsersController(ILogger<UsersController> logger, UserManager userManager, IMapper mapper)
		{
			_logger = logger;
			_userManager = userManager;
			_mapper = mapper;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<UserViewModel>> Get(string id)
		{
			var user = await _userManager.GetUser(id);
			var dto = _mapper.Map<UserViewModel>(user);
			return Ok(dto);
		}

		[HttpGet("{filter}")]
		public async Task<ActionResult<UserViewModel[]>> Search(string filter, bool includeSelf = false)
		{
			if (!(filter?.Length >= 2))
			{
				return Ok(new UserViewModel[0]);
			}

			var users = await _userManager.GetUsers(u => u.Username.ToLower().Contains(filter));
			if (!includeSelf)
			{
				users.RemoveAll(u => u.Id == this.UserId);
			}
			var dtos = _mapper.Map<UserViewModel[]>(users);
			return Ok(dtos);
		}

		public class UserLoggedInCheckResult
		{
			public UserViewModel User { get; set; }
			public bool IsFirstLogin { get; set; }
		}

		[HttpPut("{auth0Id}")]
		public async Task<ActionResult<UserLoggedInCheckResult>> LoggedIn(string auth0Id, [FromBody] JToken auth0UserJson)
		{
			ActionResult<UserLoggedInCheckResult> ToDto(User user, bool isFirstLogin)
			{
				var dto = _mapper.Map<UserViewModel>(user);
				return Ok(new UserLoggedInCheckResult { User = dto, IsFirstLogin = isFirstLogin });
			}

			var user = await _userManager.GetUserByIdentifier(auth0Id);
			if (user != null)
			{
				return ToDto(user, false);
			}

			var contractResolver = new DefaultContractResolver
			{
				NamingStrategy = new SnakeCaseNamingStrategy()
			};
			var auth0User = JsonConvert.DeserializeObject<global::Auth0.ManagementApi.Models.User>(auth0UserJson.ToString(), new JsonSerializerSettings
			{
				ContractResolver = contractResolver
			});
			var newUser = new User
			{
				Identifier = auth0Id,
				Username = auth0User.NickName,
				FirstName = auth0User.FirstName,
				LastName = auth0User.LastName,
				Avatar = auth0User.Picture,
				Email = auth0User.Email
			};

			await _userManager.CreateUser(newUser);
			return ToDto(newUser, true);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateProfile([FromBody] UserViewModel profile)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest("The User object is invalid.");
			}

			var user = new User
			{
				Id = profile.Id,
				Identifier = this.User.Auth0Id,
				Username = profile.Username,
				FirstName = profile.FirstName,
				LastName = profile.LastName,
				Avatar = profile.Avatar,
			};
			await _userManager.UpdateUser(user);
			return Ok();
		}
	}
}
