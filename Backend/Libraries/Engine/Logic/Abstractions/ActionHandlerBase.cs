using System.Collections.Generic;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;

namespace GaiaProject.Engine.Logic.Abstractions
{
	public abstract class ActionHandlerBase<T> : IHandleAction where T : PlayerAction
	{
		protected GaiaProjectGame Game;
		protected PlayerInGame Player;

		private void Initialize(GaiaProjectGame game, T action)
		{
			Game = game;
			Player = game.GetPlayer(action.PlayerId);
			InitializeImpl(game, action);
		}

		public List<Effect> Handle(GaiaProjectGame game, PlayerAction action)
		{
			return Handle(game, action as T);
		}

		private List<Effect> Handle(GaiaProjectGame game, T action)
		{
			Initialize(game, action);
			var (isValid, errorMessage) = Validate(game, action);
			if (!isValid)
			{
				throw new InvalidActionException(errorMessage);
			}
			return HandleImpl(game, action);
		}

		/// <summary>
		/// Initializes the state of the handler
		/// </summary>
		protected virtual void InitializeImpl(GaiaProjectGame game, T action) { }

		/// <summary>
		/// Actual implementation of the handler
		/// </summary>
		/// <param name="game">The game where the action was taken</param>
		/// <param name="action">The action to apply</param>
		/// <returns></returns>
		protected abstract List<Effect> HandleImpl(GaiaProjectGame game, T action);

		/// <summary>
		/// Validates the action against the current game state
		/// </summary>
		/// <param name="game">The game where the action should be taken</param>
		/// <param name="action">The action to validate</param>
		/// <returns></returns>
		protected abstract (bool isValid, string errorMessage) Validate(GaiaProjectGame game, T action);
	}
}
