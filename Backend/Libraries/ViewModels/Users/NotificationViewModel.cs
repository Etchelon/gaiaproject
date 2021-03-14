using System;
using GaiaProject.Core.Model;

namespace GaiaProject.ViewModels.Users
{
	public class NotificationViewModel
	{
		public string Id { get; set; }
		public NotificationType Type { get; protected set; }
		public DateTime Timestamp { get; set; }
		public string Text { get; set; }
		public bool IsRead { get; set; } = false;
		public NotificationViewModel()
		{
			Type = NotificationType.Generic;
		}
	}

	public class GameNotificationViewModel : NotificationViewModel
	{
		public string GameId { get; set; }

		public GameNotificationViewModel()
		{
			Type = NotificationType.Game;
		}
	}
}