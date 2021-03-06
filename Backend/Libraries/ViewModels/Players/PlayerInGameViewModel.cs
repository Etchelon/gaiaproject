using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Players
{
	public class PlayerInGameViewModel
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public Race? RaceId { get; set; }
		public bool IsActive { get; set; }
		public int? Placement { get; set; }
		public PlayerStateViewModel State { get; set; }
	}
}