using System.Linq;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;

namespace GaiaProject.Engine.Logic.Entities.Effects.Gains
{
	/// <summary>
	/// A gain changes the player's state by giving him something,
	/// be it resources or other benefits. Gains can be composite, which means
	/// that they provide multiple benefits. For example gaining a federation
	/// token may provide both resources and points
	/// </summary>
	public abstract class Gain : Effect
	{
		/// <summary>
		/// The type of gain
		/// </summary>
		public abstract GainType Type { get; }
		/// <summary>
		/// The source of the gain: either an action or another gain
		/// </summary>
		public GainSourceType SourceType { get; set; } = GainSourceType.Action;

		/// <summary>
		/// Links this gain to a composite gain
		/// </summary>
		/// <param name="gain"></param>
		public void LinkToGain(CompositeGain gain)
		{
			SourceType = GainSourceType.CompositeGain;
			Description = gain.Description;
		}

		public virtual void ApplyTo(GaiaProjectGame game, PlayerAction action)
		{
			var nullAction = new NullAction { PlayerId = PlayerId };
			var ctx = new ActionContext(nullAction, game);
			var playerState = ResourceUtils.ApplyGain(this, ctx);
			game.Players.Single(p => p.Id == PlayerId).State = playerState;
			game.LogGain(this);
		}
	}
}
