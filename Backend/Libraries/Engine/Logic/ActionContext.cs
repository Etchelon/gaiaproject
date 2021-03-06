using System.Linq;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.Logic
{
	public class ActionContext
	{
		public PlayerAction Action { get; set; }
		public GaiaProjectGame Game { get; set; }
		public PlayerInGame Player => Game.Players.FirstOrDefault(p => p.Id == Action.PlayerId);
		public PlayerState PlayerState => Player?.State;

		public ActionContext(PlayerAction action, GaiaProjectGame game)
		{
			Action = action;
			Game = game;
		}

		public void SetPlayerState(PlayerState state)
		{
			Player.State = state;
		}

		public ActionContext Clone()
		{
			return new ActionContext(Action, Game.Clone());
		}
	}
}
