using System.Linq;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;

namespace GaiaProject.Engine.Logic.Entities.Effects.Costs
{
	/// <summary>
	/// A cost is an effect that causes a reduction of a player's resources
	/// (actual resources or even other things like available gaiaformers)
	/// </summary>
	public abstract class Cost : Effect
	{
		public abstract CostType Type { get; }

		public virtual void ApplyTo(GaiaProjectGame game, PlayerAction action)
		{
			var playerState = ResourceUtils.ApplyCost(this, new ActionContext(action, game));
			game.Players.Single(p => p.Id == action.PlayerId).State = playerState;
			game.LogCost(this);
		}
	}
}
