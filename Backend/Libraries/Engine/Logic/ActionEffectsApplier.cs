using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;

namespace GaiaProject.Engine.Logic
{
	public class ActionEffectsApplier
	{
		public GaiaProjectGame ApplyActionEffects(PlayerAction action, GaiaProjectGame game, List<Effect> effects)
		{
			var playerId = action.PlayerId;
			var effectsOnPlayer = effects.Where(eff => eff.PlayerId == null).ToList();
			effectsOnPlayer.ForEach(eff =>
			{
				eff.LinkToAction(action);
				eff.ForPlayer(playerId);
			});

			var newGameState = game.Clone();
			var player = newGameState.GetPlayer(playerId);
			player.Actions.ActionPerformed(action.Type);
			if (!(action is PassTurnAction))
			{
				newGameState.LogAction(action);
			}

			effectsOnPlayer.OfType<RaceSelectedEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effectsOnPlayer.OfType<ObtainRaceWithBidEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effectsOnPlayer.OfType<ConversionEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effectsOnPlayer.OfType<Cost>().ToList().ForEach(cost => cost.ApplyTo(newGameState, action));
			effectsOnPlayer.OfType<Gain>().ToList().ForEach(gain => gain.ApplyTo(newGameState, action));
			effectsOnPlayer.OfType<HexColonizedEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effectsOnPlayer.OfType<BuildingDeployedEffect>().ToList().ForEach(eff => eff.ApplyTo(newGameState));
			effectsOnPlayer.OfType<AmbasBuildingsSwappedEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effectsOnPlayer.OfType<BescodsIncreasePowerValueOfBuildingsOnTitaniumPlanetsEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effectsOnPlayer.OfType<ChangePowerValueOfBigBuildingsEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effectsOnPlayer.OfType<AcquireTechnologyTileEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			newGameState = effectsOnPlayer.OfType<RawStateChangeEffect>().Aggregate(
				newGameState,
				(gameState, rsc) =>
				{
					var (ret, message) = rsc.ChangeFn(new ActionContext(action, gameState));
					return ret;
				}
			);
			effectsOnPlayer.OfType<AcquireRoundBoosterEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effectsOnPlayer.OfType<FederationCreatedEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effectsOnPlayer.OfType<IvitsFederationExpandedEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effectsOnPlayer.OfType<EnlargeFederationEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effectsOnPlayer.OfType<ChangePowerValueOfFederationEffect>().ToList().ForEach(eff => eff.ApplyTo(newGameState));
			effectsOnPlayer.OfType<MergeFederationsEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effectsOnPlayer.OfType<IncomeVariationEffect>().ToList().ForEach(eff => eff.ApplyTo(newGameState));

			// Apply effects to other players, such as decisions and power charging
			var effectsOnOtherPlayers = effects
				.Where(eff => eff.PlayerId != playerId)
				.ToList();
			effectsOnOtherPlayers.OfType<Gain>().ToList().ForEach(eff =>
			{
				eff.LinkToAction(action);
				eff.ApplyTo(newGameState, action);
			});

			effects.OfType<TechnologyTrackBonusTakenEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effects.OfType<PowerActionUsedEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effects.OfType<QicActionUsedEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effects.OfType<SpecialActionUsedEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effects.OfType<FederationTokenTakenEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effects.OfType<PendingDecisionEffect>().ToList().ForEach(eff => eff.ApplyTo(newGameState));
			effects.OfType<ClearTempStatsEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effects.OfType<LogEffect>().ToList().ForEach(eff => eff.ApplyTo(newGameState));
			effects.OfType<PlayerPassedEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effects.OfType<NextTurnEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effects.OfType<PassTurnToPlayerEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effects.OfType<AuctionEndedEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effects.OfType<GotoSubphaseEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			effects.OfType<StartRoundsPhaseEffect>().SingleOrDefault()?.ApplyTo(newGameState);
			return newGameState;
		}
	}
}
