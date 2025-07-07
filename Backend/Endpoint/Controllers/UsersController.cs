using System;
using System.Threading.Tasks;
using AutoMapper;
using GaiaProject.Core.Logic;
using GaiaProject.Core.Model;
using GaiaProject.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GaiaProject.Endpoint.Controllers;

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
			users.RemoveAll(u => u.IdStr == UserId);
		}
		var dtos = _mapper.Map<UserViewModel[]>(users);
		return Ok(dtos);
	}

	[HttpGet]
	public async Task<ActionResult<Notification[]>> CountUnreadNotifications()
	{
		var count = await _userManager.CountUnreadNotifications(UserId);
		return Ok(count);
	}

	[HttpGet]
	public async Task<ActionResult<NotificationViewModel[]>> GetUserNotifications(DateTime earlierThan, bool alsoUnread)
	{
		var notifications = await _userManager.GetUserNotifications(UserId, earlierThan);
		var dtos = _mapper.Map<NotificationViewModel[]>(notifications);
		return Ok(dtos);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> SetNotificationRead(string id)
	{
		await _userManager.SetNotificationRead(UserId, id);
		return NoContent();
	}
}
