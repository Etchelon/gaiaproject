using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Players
{
	public class PlayerInfoViewModel
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public string AvatarUrl { get; set; }
		public Race? RaceId { get; set; }
		public string RaceName { get; set; }
		public string Color { get; set; }
		public int Points { get; set; }
		public bool IsActive { get; set; }
		public int? Placement { get; set; }
	}
}