using System;
using GaiaProject.Engine.Enums;
using GaiaProject.ViewModels.Users;

namespace GaiaProject.ViewModels
{
	public abstract class GameBaseViewModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public UserViewModel CreatedBy { get; set; }
		public DateTime Created { get; set; }
		public DateTime? Ended { get; set; }
		public GamePhase CurrentPhase { get; set; }
	}
}