using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class PassActionHandler : ActionHandlerBase<PassAction>
	{
		private bool _isLastRound;

		protected override void InitializeImpl(GaiaProjectGame game, PassAction action)
		{
			_isLastRound = game.Rounds.CurrentRound == GaiaProjectGame.LastRound;
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, PassAction action)
		{
			var effects = new List<Effect>();
			var returnedBooster = Player.State.RoundBooster.Id;
			var pointsUponPassing = PointUtils.GetPointsUponPassing(Player.Id, game);
			effects.AddRange(pointsUponPassing);

			if (!_isLastRound)
			{
				var acquiredBooster = action.SelectedRoundBooster!.Value;
				effects.Add(new AcquireRoundBoosterEffect(acquiredBooster, returnedBooster));
				effects.Add(new IncomeVariationEffect(IncomeSource.RoundBooster));
			}
			effects.Add(new PlayerPassedEffect());
			if (!game.Players.Where(p => p.Id != Player.Id).All(p => p.HasPassed))
			{
				var nextPlayer = TurnOrderUtils.GetNextPlayer(Player.Id, game, true);
				effects.Add(new PassTurnToPlayerEffect(nextPlayer));
			}
			else if (!_isLastRound)
			{
				effects.Add(new NextTurnEffect());
			}

			// If the player forgot to convert Gaiaformers to Qic before passing, do it here
			if (Player.RaceId == Race.BalTaks && Player.State.Gaiaformers.Any(gf => gf.Unlocked && gf.Available))
			{
				var nAvailableGaiaformers = Player.State.Gaiaformers.Count(gf => gf.Unlocked && gf.Available);
				effects.Add(new ConversionEffect(0, 0, 0, nAvailableGaiaformers, 0, 0, null, 0, 0, nAvailableGaiaformers, 0));
			}
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, PassAction action)
		{
			if (Player.HasPassed)
			{
				return (false, "You have already passed");
			}
			if (ShouldSelectedRoundBooster(action))
			{
				return (false, "You have not selected a round booster");
			}
			if (!_isLastRound && SelectedRoundBoosterIsTaken(action.SelectedRoundBooster!.Value))
			{
				return (false, "The round booster you selected is already taken");
			}
			return (true, null);
		}

		#region Validation

		private bool ShouldSelectedRoundBooster(PassAction action)
		{
			return !_isLastRound && !action.SelectedRoundBooster.HasValue;
		}

		private bool SelectedRoundBoosterIsTaken(RoundBoosterType type)
		{
			var booster = Game.BoardState.RoundBoosters.AvailableRoundBooster.Single(b => b.Id == type);
			return booster.IsTaken;
		}

		#endregion
	}
}
