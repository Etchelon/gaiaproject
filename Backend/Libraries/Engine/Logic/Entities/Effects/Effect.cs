using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	/// <summary>
	/// Base class for all effects caused by an action.
	/// </summary>
	public abstract class Effect
	{
		/// <summary>
		/// The action id that generated the effect
		/// </summary>
		public int? ActionId { get; set; }
		/// <summary>
		/// The description of the effect
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// The player that is the target of the effect. NULL if it's the acting player
		/// </summary>
		public string PlayerId { get; set; }
		/// <summary>
		/// Whether this effect should be logged or ignored
		/// </summary>
		public bool Loggable { get; protected set; } = true;

		/// <summary>
		/// Links this effect to an action
		/// </summary>
		/// <param name="actionId"></param>
		/// <param name="description"></param>
		public virtual void LinkToAction(PlayerAction action, string description = null)
		{
			ActionId = action.Id;
			Description = description;
		}

		/// <summary>
		/// If the target of the effect is not the acting player, specify who it is
		/// </summary>
		/// <param name="playerId"></param>
		public virtual void ForPlayer(string playerId)
		{
			PlayerId = playerId;
		}

		public virtual void ApplyTo(GaiaProjectGame game) { }
	}
}
