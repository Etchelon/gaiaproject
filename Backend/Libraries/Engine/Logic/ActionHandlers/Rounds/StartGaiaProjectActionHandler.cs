using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class StartGaiaProjectActionHandler : ActionHandlerBase<StartGaiaProjectAction>
	{
		private ActionContext _ctx;
		private Hex _targetHex;
		private MapService _mapService;
		private bool _isInRange;
		private int _requiredQics = 0;

		protected override void InitializeImpl(GaiaProjectGame game, StartGaiaProjectAction action)
		{
			_ctx = new ActionContext(action, game);
			_targetHex = game.BoardState.Map.Hexes.First(h => h.Id == action.HexId);
			_mapService = new MapService(game.BoardState.Map);
			_isInRange = IsInRange();
		}

		protected override List<Effect> HandleImpl(GaiaProjectGame game, StartGaiaProjectAction action)
		{
			var effects = new List<Effect>
			{
				GetActionCost(),
				new BuildingDeployedEffect(BuildingType.Gaiaformer, _targetHex.Id),
				new ClearTempStatsEffect(),
				new PendingDecisionEffect(new PerformConversionOrPassTurnDecision()),
			};
			if (this._requiredQics > 0)
			{
				effects.Insert(0, new ResourcesCost(new Resources { Qic = this._requiredQics }));
			}
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, StartGaiaProjectAction action)
		{
			if (!HasAvailableGaiaformers())
			{
				return (false, "You don't have any available gaiaformers");
			}
			if (!HexIsTransdim())
			{
				return (false, "The selected hex is already taken");
			}
			if (HexIsTaken())
			{
				return (false, "The selected hex is already taken");
			}
			if (!_isInRange)
			{
				return (false, "The selected hex is not in range");
			}
			if (!HasEnoughPowerTokens(out _))
			{
				return (false, "You do not have the required power tokens");
			}
			return (true, null);
		}

		#region Validation

		private bool HasEnoughPowerTokens(out string reason)
		{
			var cost = GetActionCost();
			return ResourceUtils.CanPayCost(cost, _ctx, out reason);
		}

		private bool IsInRange()
		{
			var (hex, requiredQics) = _mapService.GetHexesReachableBy(Player).SingleOrDefault(o => o.hex.Id == _targetHex.Id);
			if (hex == null)
			{
				return false;
			}

			this._requiredQics = requiredQics;
			return Player.State.Resources.Qic >= requiredQics;
		}

		private bool HexIsTaken()
		{
			return _targetHex.Buildings.Any();
		}

		private bool HexIsTransdim()
		{
			return _targetHex.PlanetType == PlanetType.Transdim && !(_targetHex.WasGaiaformed ?? false);
		}

		private bool HasAvailableGaiaformers()
		{
			return Player.State.Gaiaformers.Any(gf => gf.Unlocked && gf.Available);
		}

		#endregion

		private PowerTokensCost GetActionCost()
		{
			return GaiaformingUtils.GetActualCostForGaiaProject(Player);
		}
	}
}